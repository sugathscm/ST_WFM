using System;
using System.Collections.Generic;
using System.Linq;
using WFM.BAL.Enums;
using WFM.DAL;

namespace WFM.BAL.Services
{
    public class OrderService
    {
        ErrorLogService errlog = new ErrorLogService();
        public List<Order> GetOrderList()
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                return entities.Orders.Include("Client").OrderBy(d => d.Id).ToList();
            }
        }
        public List<DeliveryType> GetDeliveryTypeList()
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                return entities.DeliveryTypes.ToList();
            }
        }

        public List<Illumination> GetIlluminationList()
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                return entities.Illuminations.ToList();
            }
        }
        public List<Order> GetOrderActiveList()
        {
            try
            {
                using (DB_stwfmEntities entities = new DB_stwfmEntities())
                {
                    return entities.Orders
                        .Include("Client")
                        .Include("DeliveryType")
                        .Include("OrderType")
                        .Include("Employee").Where(o => o.StatusId != (int)OrderStatus.Completed).OrderBy(d => d.Id).ToList();

                }
            } catch(Exception er) 
            {
                return null;
            }
        }

        public List<Order> GetOrderHistoryList()
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                return entities.Orders.Include("Client").Where(o => o.StatusId == (int)OrderStatus.Completed).OrderBy(d => d.Id).ToList();
            }
        }

        public Order GetOrderById(int? id)
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                return entities.Orders
                    .Include("OrderItems")
                    .Include("Client")
                    .Include("OrderItems.Category")
                    .Include("Employee")
                     .Include("Quote")
                    .Include("AdditionalCharges").Where(s => s.Id == id).SingleOrDefault();
            }
        }

        public void SaveOrUpdate(Order order)
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                if (order.Id == 0)
                {
                    entities.Orders.Add(order); 
                    entities.SaveChanges();
                }
                else
                {
                    entities.Entry(order).State = System.Data.Entity.EntityState.Modified;
                    entities.SaveChanges();
                }
            }
        }

        public void SaveOrUpdate(OrderItem orderItem)
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                if (orderItem.OrderId != 0)
                {
                    entities.OrderItems.Add(orderItem);
                    entities.SaveChanges();
                }
            }
        }
        public void SaveOrUpdate(AdditionalCharge additionalCharge)
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                if (additionalCharge.Id != 0)
                {
                    entities.AdditionalCharges.Add(additionalCharge);
                    entities.SaveChanges();
                }
               

            }
        }

        public void SaveOrUpdate(Quote model, string userId, string wayforward)
        {
            string newData = string.Empty, oldData = string.Empty;

            try
            {
                using (DB_stwfmEntities entities = new DB_stwfmEntities())
                {
                    int id = model.Id;
                    Order order = null;

                    string OrderCode = CommonService.GenerateOrderCode(DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString("00"), model.IsVAT, model.Code);

                    order = new Order
                    {
                        ClientId = (int)model.ClientId,
                        Code = OrderCode,
                        CodeNumber = int.Parse(OrderCode.Split('/')[3]),
                        Year = DateTime.Now.Year.ToString(),
                        Month = DateTime.Now.Month.ToString("00"),
                        ChanneledById = (int)model.ChanneledById,
                        Value = model.Value,
                        Comments = model.Comments,
                        CreatedDate = DateTime.Now,
                        CreatedBy = userId,
                        Header = model.Header,
                        IsVAT = model.IsVAT,
                        ContactPerson = model.ContactPerson,
                        ContactMobile = model.ContactMobile
                    };

                    OrderItem orderItem = null;

                    foreach (var item in model.QuoteItems)
                    {
                        orderItem = new OrderItem
                        {
                            CategoryId = item.CategoryId,
                            Qty = (int)item.Qty,
                            UnitCost = (double)item.UnitCost,
                            VAT = (double)item.VAT,
                            Size = item.Size,
                            Description = item.Description,
                            CategoryName = item.CategoryName,
                            CategoryType = item.CategoryType,
                            VisibilityId = item.VisibilityId,
                            FrameworkWarrantyPeriod = (int)item.FrameworkWarrantyPeriod,
                            IlluminationWarrantyPeriod = (int)item.IlluminationWarrantyPeriod,
                            LetteringWarrantyPeriod = (int)item.LetteringWarrantyPeriod,
                        
                        };

                        order.OrderItems.Add(orderItem);
                    }

                    var arrWayForward = wayforward.Split(',');
                    arrWayForward = arrWayForward.Where(val => val != "").ToArray();

                    foreach (var item in arrWayForward)
                    {
                        OrderWayForward orderWayForward = new OrderWayForward
                        {
                            DivisionId = int.Parse(item)
                        };
                        //rder.OrderWayForwards.Add(orderWayForward);
                    }

                    entities.Orders.Add(order);
                    entities.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ErrorLog er = new ErrorLog();
                er.Type = "Order";
                er.Error = ex.Message;
                errlog.SaveOrUpdate(er);
            }
        }

        public void RemoveItems(Order order)
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                bool validateOnSaveEnabled = entities.Configuration.ValidateOnSaveEnabled;
                try
                {
                    entities.Configuration.ValidateOnSaveEnabled = false;
                    foreach (var item in order.OrderItems.ToList())
                    {
                        entities.OrderItems.Attach(item);
                        entities.OrderItems.Remove(item);
                        entities.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                    }
                    entities.SaveChanges();
                }
                catch (Exception ex)
                {
                    string err = ex.ToString();
                    throw;
                }
                finally
                {
                    entities.Configuration.ValidateOnSaveEnabled = validateOnSaveEnabled;
                }
            }
        }
    }
}