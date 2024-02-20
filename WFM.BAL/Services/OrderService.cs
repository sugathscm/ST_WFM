using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Runtime.Remoting.Contexts;
using WFM.BAL.Enums;
using WFM.DAL;
using Attribute = WFM.DAL.Attribute;

namespace WFM.BAL.Services
{
    public class OrderService
    {
        private readonly MemoryCache _memoryCache;

        public OrderService()
        {
            _memoryCache = new MemoryCache("STWFM");
        }

        public string GetIllumination(int? Id = 0)
        {
            if (_memoryCache.Get("luminatio") != null)
            {
                var ill = (List<Illumination>)_memoryCache.Get("luminatio");
                var IllName = ill.Where(e => e.Id == Id).FirstOrDefault();
                return IllName.Name == null ? "" : IllName.Name;

            }
            else
            {

                using (DB_stwfmEntities entities = new DB_stwfmEntities())
                {
                    var ill = (List<Illumination>)entities.Illuminations.ToList();
                    _memoryCache.Add("luminatio", ill, null);
                    var illName = ill.Where(e => e.Id == Id).FirstOrDefault();
                    return illName.Name == null ? "" : illName.Name;
                }
            }

        }

        public string GetWarranty(int? Id = 4)
        {
            if (_memoryCache.Get("warranty") != null)
            {
                var war = (List<WarrantyPeriod>)_memoryCache.Get("warranty");
                var warName = war.Where(e => e.Id == Id).FirstOrDefault();
                return warName.Duration == null ? "" : warName.Duration;

            }
            else
            {
                using (DB_stwfmEntities entities = new DB_stwfmEntities())
                {
                    var war = (List<WarrantyPeriod>)entities.WarrantyPeriods.ToList();
                    _memoryCache.Add("warranty", war, null);
                    var warName = war.Where(e => e.Id == Id).FirstOrDefault();
                    return warName.Duration == null ? "" : warName.Duration;
                }

            }


        }

        public string Getvisibility(int? Id = 3)
        {
            if (_memoryCache.Get("visibility") != null)
            {
                var war = (List<Visibility>)_memoryCache.Get("visibility");
                var warName = war.Where(e => e.Id == Id).FirstOrDefault();
                return warName.Name == null ? "" : warName.Name;

            }
            else
            {
                using (DB_stwfmEntities entities = new DB_stwfmEntities())
                {
                    var war = (List<Visibility>)entities.Visibilities.ToList();
                    _memoryCache.Add("visibility", war, null);
                    var warName = war.Where(e => e.Id == Id).FirstOrDefault();
                    return warName.Name == null ? "" : warName.Name;
                }

            }


        }

        ErrorLogService errlog = new ErrorLogService();
     
        public List<DeliveryType> GetDeliveryTypeList()
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                return entities.DeliveryTypes.ToList();
            }
        }
        public List<Material> GetMaterialList()
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                return entities.Materials.ToList();
            }
        }
        public List<Attribute> GetAttributeList()
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                return entities.Attributes.ToList();
            }
        }

        public List<Supplier> GetSupplierList()
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                return entities.Suppliers.ToList();
            }
        }
        public void DeleteOrderAttachment(int id)
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                var orderAttachment = entities.OrderAttachments.Include("Order").FirstOrDefault(o => o.Id == id);

                if (orderAttachment != null)
                {

                    entities.OrderAttachments.Remove(orderAttachment);
                    entities.SaveChanges();
                }
            }
        }
        public void DeleteInstallationAttachment(int id)
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                var orderAttachment = entities.InstallationAttachments.Include("Order").FirstOrDefault(o => o.Id == id);

                if (orderAttachment != null)
                {

                    entities.InstallationAttachments.Remove(orderAttachment);
                    entities.SaveChanges();
                }
            }
        }
        public void CancelOrder(int id)
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {

                var order = entities.Orders.FirstOrDefault(x => x.Id == id);
                order.isCancelled = true;
                entities.SaveChanges();
            }
        }

        public string GetDeliveryType(int Id)
        {
            if (_memoryCache.Get("deltype") != null)
            {
                var war = (List<DeliveryType>)_memoryCache.Get("deltype");
                var warName = war.Where(e => e.Id == Id).FirstOrDefault();
                return warName.Type == null ? "" : warName.Type;

            }
            else
            {
                using (DB_stwfmEntities entities = new DB_stwfmEntities())
                {
                    var war = (List<DeliveryType>)entities.DeliveryTypes.ToList();
                    _memoryCache.Add("deltype", war, null);
                    var warName = war.Where(e => e.Id == Id).FirstOrDefault();
                    return warName.Type == null ? "" : warName.Type;
                }

            }
            //using (DB_stwfmEntities entities = new DB_stwfmEntities())
            //{
            //    var del=entities.DeliveryTypes.Where(e => e.Id == Id).FirstOrDefault();
            //    return del.Type;
            //}
        }

        public List<Visibility> GetVisibilityList()
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                return entities.Visibilities.OrderBy(l => l.Name).ToList();
            }
        }


        public List<Illumination> GetIlluminationList()
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                return entities.Illuminations.ToList();
            }
        }
        public List<Order> GetOrderList()
        {
            try
            {
                using (DB_stwfmEntities entities = new DB_stwfmEntities())
                {
                    return entities.Orders
                        .Include("OrderItems")
                        .Include("InstallationAttachments")
                        .Include("AdditionalCharges")
                        .Include("Client")
                        .Include("DeliveryType")
                        .Include("Status")
                        .Include("OrderType")
                        .Include("Employee").Where(o => o.isCancelled == false).OrderByDescending(d => d.CreatedDate).ToList();

                }
            }
            catch (Exception er)
            {
                return null;
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
                        .Include("Status")
                        .Include("OrderType")
                        .Include("Employee").Where(o => o.StatusId != (int)OrderStatus.Completed && o.isCancelled == false).OrderByDescending(d => d.CreatedDate).ToList();

                }
            }
            catch (Exception er)
            {
                return null;
            }
        }

        public List<Order> GetOrderCompleteList()
        {
            try
            {
                using (DB_stwfmEntities entities = new DB_stwfmEntities())
                {
                    return entities.Orders
                        .Include("Client")
                        .Include("DeliveryType")
                        .Include("Status")
                        .Include("OrderType")
                        .Include("Employee").Where(o => o.StatusId == (int)OrderStatus.Completed && o.isCancelled == false).OrderByDescending(d => d.CreatedDate).ToList();

                }
            }
            catch (Exception er)
            {
                return null;
            }
        }
        public List<Order> GetOrderCancelledList()
        {
            try
            {
                using (DB_stwfmEntities entities = new DB_stwfmEntities())
                {
                    return entities.Orders
                        .Include("Client")
                        .Include("DeliveryType")
                        .Include("Status")
                        .Include("OrderType")
                        .Include("Employee").Where(o => o.isCancelled == true).OrderByDescending(d => d.CreatedDate).ToList();

                }
            }
            catch (Exception er)
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
                    .Include("OrderMaterials.Supplier")
                    .Include("OrderMaterials.Material")
                    .Include("OrderAttachments")
                    .Include("InstallationAttachments")
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
                if (orderItem.Id == 0)
                {
                    entities.OrderItems.Add(orderItem);
                    entities.SaveChanges();
                }
                else
                {
                    entities.Entry(orderItem).State = System.Data.Entity.EntityState.Modified;
                    entities.SaveChanges();
                }
            }
        }
        public void SaveOrUpdate(OrderMaterial orderMaterial)
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                if (orderMaterial.Id == 0)
                {
                    entities.OrderMaterials.Add(orderMaterial);
                    entities.SaveChanges();
                }
                else
                {
                    entities.Entry(orderMaterial).State = System.Data.Entity.EntityState.Modified;
                    entities.SaveChanges();
                }
            }
        }

        public void SaveOrUpdate(OrderAttachment orderAttachment)
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {

                entities.OrderAttachments.Add(orderAttachment);
                entities.SaveChanges();

            }
        }
        public void SaveOrUpdate(InstallationAttachment installationAttachment)
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {

                entities.InstallationAttachments.Add(installationAttachment);
                entities.SaveChanges();

            }
        }
        public void SaveOrUpdate(AdditionalCharge additionalCharge)
        {
            using (DB_stwfmEntities entities = new DB_stwfmEntities())
            {
                if (additionalCharge.Id == 0)
                {
                    entities.AdditionalCharges.Add(additionalCharge);
                    entities.SaveChanges();
                }
                else
                {
                    entities.Entry(additionalCharge).State = System.Data.Entity.EntityState.Modified;
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