using System;
using System.Collections.Generic;
using System.Linq;
using WFM.BAL.Enums;
using WFM.DAL;

namespace WFM.BAL.Services
{
    public class OrderService
    {
        public List<Order> GetOrderList()
        {
            using (DB_A4EFEA_stwfmEntities entities = new DB_A4EFEA_stwfmEntities())
            {
                return entities.Orders.Include("Client").OrderBy(d => d.Id).ToList();
            }
        }

        public List<Order> GetOrderActiveList()
        {
            using (DB_A4EFEA_stwfmEntities entities = new DB_A4EFEA_stwfmEntities())
            {
                return entities.Orders.Include("Client").Where(o => o.StatusId != (int)OrderStatus.Completed).OrderBy(d => d.Id).ToList();
            }
        }

        public List<Order> GetOrderHistoryList()
        {
            using (DB_A4EFEA_stwfmEntities entities = new DB_A4EFEA_stwfmEntities())
            {
                return entities.Orders.Include("Client").Where(o => o.StatusId == (int)OrderStatus.Completed).OrderBy(d => d.Id).ToList();
            }
        }

        public Order GetOrderById(int? id)
        {
            using (DB_A4EFEA_stwfmEntities entities = new DB_A4EFEA_stwfmEntities())
            {
                return entities.Orders.Include("OrderItems").Where(s => s.Id == id).SingleOrDefault();
            }
        }

        public void SaveOrUpdate(Order order)
        {
            using (DB_A4EFEA_stwfmEntities entities = new DB_A4EFEA_stwfmEntities())
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
            using (DB_A4EFEA_stwfmEntities entities = new DB_A4EFEA_stwfmEntities())
            {
                if (orderItem.OrderId != 0)
                {
                    entities.OrderItems.Add(orderItem);
                    entities.SaveChanges();
                }
            }
        }

        public void SaveOrUpdate(Quote model, string userId, string wayforward)
        {
            string newData = string.Empty, oldData = string.Empty;

            try
            {
                using (DB_A4EFEA_stwfmEntities entities = new DB_A4EFEA_stwfmEntities())
                {
                    int id = model.Id;
                    Order order = null;

                    string OrderCode = CommonService.GenerateOrderCode(DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString("00"), model.IsVAT, model.Code);

                    order = new Order
                    {
                        ClientId = model.ClientId,
                        Code = OrderCode,
                        CodeNumber = int.Parse(OrderCode.Split('/')[3]),
                        Year = DateTime.Now.Year.ToString(),
                        Month = DateTime.Now.Month.ToString("00"),
                        ChanneledBy = model.ChanneledBy,
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
                            Qty = item.Qty,
                            UnitCost = item.UnitCost,
                            VAT = item.VAT,
                            Size = item.Size,
                            Description = item.Description,
                            CategoryName = item.CategoryName,
                            CategoryType = item.CategoryType,
                            FrameworkWarrantyPeriod = item.FrameworkWarrantyPeriod,
                            IlluminationWarrantyPeriod = item.IlluminationWarrantyPeriod,
                            LetteringWarrantyPeriod = item.LetteringWarrantyPeriod,
                            VisibilityId = item.VisibilityId
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
            }
        }

        public void RemoveItems(Order order)
        {
            using (DB_A4EFEA_stwfmEntities entities = new DB_A4EFEA_stwfmEntities())
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