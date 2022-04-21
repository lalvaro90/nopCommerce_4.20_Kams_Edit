using DataLibrary;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;

namespace Kams_Sync_App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        bool keepExecutionOrders = true;
        bool keepExecutionProducts = true;
        TimeSpan orderDelay = TimeSpan.FromMinutes(2);
        TimeSpan ProductDelay = TimeSpan.FromDays(1);
        Thread OrdersTh, ProductsTh;

        public MainWindow()
        {
            try
            {
                InitializeComponent();
                var ct = SynchronizationContext.Current;
                OrdersTh = new Thread((object value) =>
                {
                    var ctx = (SynchronizationContext)value;
                    while (keepExecutionOrders)
                    {
                        Thread.Sleep(orderDelay);
                        Thread sth = new Thread(InitSyncOrders);
                        ctx.Send(clearOrderLogText, string.Empty);
                        sth.Start(ctx);
                    }
                });
                OrdersTh.Start(ct);

                ProductsTh = new Thread((object value) =>
                {
                    var ctx = (SynchronizationContext)value;
                    while (keepExecutionProducts)
                    {
                        Thread.Sleep(ProductDelay);
                        Thread sth = new Thread(InitSyncProducts);
                        ctx.Send(clearProductLogText, string.Empty);
                        sth.Start(ctx);
                    }
                });
                ProductsTh.Start(ct);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(this.ToString(), ex.ToString());
                throw;
            }
            


        }


        void clearOrderLogText(object value)
        {
            txt_orders_log.Text = string.Empty;
        }

        void clearProductLogText(object value)
        {
            txt_orders_log.Text = string.Empty;
        }

        void setOrderProgressInitialValues(object maxValue)
        {
            int value = (int)maxValue;
            prg_ProgressOrders.Maximum = value;
            prg_ProgressOrders.Minimum = 0;
            prg_ProgressOrders.Value = 0;
            lbl_progressOrders.Content = $"0 de {value}";
        }

        void setProductProgressInitialValues(object maxValue)
        {
            int value = (int)maxValue;
            prg_ProgressProducts.Maximum = value;
            prg_ProgressProducts.Minimum = 0;
            prg_ProgressProducts.Value = 0;
            lbl_progressProducts.Content = $"0 de {value}";
        }
        void increaseOrderProgressValues(object maxValue)
        {
            prg_ProgressOrders.Value++;
            lbl_progressOrders.Content = $"{prg_ProgressOrders.Value} de {prg_ProgressOrders.Maximum}";
        }
        void increaseProductProgressValues(object maxValue)
        {
            prg_ProgressProducts.Value++;
            lbl_progressProducts.Content = $"{prg_ProgressProducts.Value} de {prg_ProgressProducts.Maximum}";
        }

        void writeToOrderProgress(object log)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(txt_orders_log.Text);
            sb.AppendLine((string)log);
            txt_orders_log.Text = sb.ToString();
            sv_orders.ScrollToEnd();
            LogHelper.WriteToLog((string)log);
        }

        void writeToProductProgress(object log)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(txt_products_log.Text);
            sb.AppendLine((string)log);
            txt_products_log.Text = sb.ToString();
            sv_products.ScrollToEnd();
            LogHelper.WriteToLog((string)log);
        }

        void InitSyncProducts(object ctx)
        {

            SynchronizationContext ctxx = (SynchronizationContext)ctx;

            try
            {
                kamscrin_npcommerceEntities storeEntities = new kamscrin_npcommerceEntities();
                ctxx.Send(writeToProductProgress, "Opteniendo Lista de Grupos");
                List<Group> categories = SQLHelper.getCategories();
                int num1 = 0;
                ctxx.Send(writeToProductProgress, string.Format("Procesando {0} Categorias", (object)categories.Count));
                ctxx.Send(setProductProgressInitialValues, (object)categories.Count);
                foreach (Group group in categories)
                {
                    Group g = group;
                    ctxx.Send(writeToProductProgress, $"Categoria -> {g.SDescripcion} - {g.SGrupo}");
                    Console.WriteLine();
                    Category ct = new Category()
                    {
                        Name = g.SDescripcion,
                        Description = g.SDescripcion,
                        MetaDescription = g.SGrupo,
                        MetaKeywords = g.SGrupo_Padre,
                        PageSize = 10
                    };
                    Category category = ((IQueryable<Category>)storeEntities.Categories).FirstOrDefault<Category>(x => x.MetaDescription == g.SGrupo_Padre);
                    Category exist = ((IQueryable<Category>)storeEntities.Categories).FirstOrDefault<Category>(x => x.MetaDescription == g.SGrupo);
                    if (exist == null)
                    {
                        if (category != null)
                            num1 = category.Id;
                        ct.ParentCategoryId = num1;
                        storeEntities.Categories.Add(ct);
                        storeEntities.SaveChanges();
                        if (((IQueryable<UrlRecord>)storeEntities.UrlRecords).FirstOrDefault<UrlRecord>(x => x.EntityId == ct.Id && x.EntityName == "Category") == null)
                        {
                            storeEntities.UrlRecords.Add(new UrlRecord()
                            {
                                EntityId = ct.Id,
                                EntityName = "Category",
                                Slug = RemoveSpecialCharacters(ct.Name),
                                IsActive = true
                            });
                            storeEntities.SaveChanges();
                        }
                    }
                    else
                    {
                        exist.Description = g.SDescripcion;
                        exist.Name = g.SDescripcion;
                        exist.MetaDescription = g.SGrupo;
                        exist.MetaKeywords = g.SGrupo_Padre;
                        exist.PageSize = 10;
                        storeEntities.SaveChanges();
                        if (((IQueryable<UrlRecord>)storeEntities.UrlRecords).FirstOrDefault<UrlRecord>(x => x.EntityId == exist.Id && x.EntityName == "Category") == null)
                        {
                            storeEntities.UrlRecords.Add(new UrlRecord()
                            {
                                EntityId = exist.Id,
                                EntityName = "Category",
                                Slug = RemoveSpecialCharacters(exist.Name),
                                IsActive = true
                            });
                            storeEntities.SaveChanges();
                        }
                    }
                    ctxx.Send(increaseProductProgressValues, group);
                }
                ctxx.Send(writeToProductProgress, $"Obteniendo Prductos");
                List<PCGRAF_Product> products = SQLHelper.getProducts();
                ctxx.Send(writeToProductProgress, string.Format("Procesando {0} Productos", products.Count));
                ctxx.Send(setProductProgressInitialValues, products.Count);
                int num2 = 0;
                foreach (PCGRAF_Product p in products)
                {
                    ctxx.Send(writeToProductProgress, $"Prducto { p.SCodigo_Producto} - P1: {p.P1} - P2: {p.P2} - P3: {p.P3} - P4: {p.P4}");
                    try
                    {
                    label_37:
                        Decimal P1 = 0;
                        Decimal P2 = 0;
                        Decimal P3 = 0;
                        Decimal P4 = 0;
                        if (p.P1 != null)
                            Decimal.TryParse(p.P1, out P1);
                        if (p.P2 != null)
                            Decimal.TryParse(p.P2, out P2);
                        if (p.P3 != null)
                            Decimal.TryParse(p.P3, out P3);
                        if (p.P4 != null)
                            Decimal.TryParse(p.P4, out P4);
                        Category exist_cat = storeEntities.Categories.FirstOrDefault<Category>(x => x.MetaDescription == p.SGrupo);
                        Product product = new Product()
                        {
                            Name = p.SDescripcion_Inventario,
                            ShortDescription = p.SDescripcion_Inventario,
                            Price1 = new Decimal?(P1),
                            Price2 = new Decimal?(P2),
                            Price3 = new Decimal?(P3),
                            Price4 = new Decimal?(P4),
                            MetaKeywords = p.SCodigo_Producto,
                            Sku = p.SCodigo_Producto,
                            ProductTypeId = 5,
                            CreatedOnUtc = DateTime.UtcNow,
                            StockQuantity = 9999,
                            VisibleIndividually = true,
                            OrderMaximumQuantity = 999,
                            OrderMinimumQuantity = 1
                        };
                        Product exist_prod = storeEntities.Products.FirstOrDefault<Product>(x => x.MetaKeywords == p.SCodigo_Producto);
                        if (exist_prod != null)
                        {
                            Product_Category_Mapping productCategoryMapping = storeEntities.Product_Category_Mapping.FirstOrDefault<Product_Category_Mapping>(x => x.CategoryId == exist_cat.Id && x.ProductId == exist_prod.Id);
                            if (productCategoryMapping == null && exist_prod != null)
                            {
                                storeEntities.Product_Category_Mapping.Add(new Product_Category_Mapping()
                                {
                                    CategoryId = exist_cat.Id,
                                    ProductId = exist_prod.Id
                                });
                                storeEntities.SaveChanges();
                            }
                            else if (productCategoryMapping != null && exist_prod != null)
                            {
                                productCategoryMapping.ProductId = exist_prod.Id;
                                productCategoryMapping.CategoryId = exist_cat.Id;
                                exist_prod.Price1 = new Decimal?(Decimal.Parse(p.P1));
                                exist_prod.Price2 = new Decimal?(Decimal.Parse(p.P2));
                                exist_prod.Price3 = new Decimal?(Decimal.Parse(p.P3));
                                exist_prod.Price4 = new Decimal?(Decimal.Parse(p.P4));
                                exist_prod.Name = p.SDescripcion_Inventario;
                                exist_prod.ShortDescription = p.SDescripcion_Inventario;
                                exist_prod.MetaKeywords = p.SCodigo_Producto;
                                exist_prod.OrderMinimumQuantity = 1;
                                exist_prod.OrderMaximumQuantity = 999;
                                exist_prod.ProductTypeId = 5;
                                storeEntities.SaveChanges();
                            }
                            else
                            {
                                storeEntities.Products.Add(product);
                                storeEntities.SaveChanges();
                                Thread.Sleep(500);
                                storeEntities.Product_Category_Mapping.Add(new Product_Category_Mapping()
                                {
                                    CategoryId = exist_cat.Id,
                                    ProductId = product.Id
                                });
                                storeEntities.SaveChanges();
                            }
                            if (storeEntities.UrlRecords.FirstOrDefault<UrlRecord>(x => x.EntityId == exist_prod.Id && x.EntityName == "Product") == null)
                            {
                                storeEntities.UrlRecords.Add(new UrlRecord()
                                {
                                    EntityId = exist_prod.Id,
                                    EntityName = "Product",
                                    Slug = RemoveSpecialCharacters(exist_prod.Name),
                                    IsActive = true
                                });
                                storeEntities.SaveChanges();
                            }
                        }
                        else {
                            storeEntities.Products.Add(product);
                            storeEntities.SaveChanges();
                            storeEntities.UrlRecords.Add(new UrlRecord()
                            {
                                EntityId = product.Id,
                                EntityName = "Product",
                                Slug = RemoveSpecialCharacters(p.SDescripcion_Inventario),
                                IsActive = true
                            });
                            storeEntities.Product_Category_Mapping.Add(new Product_Category_Mapping()
                            {
                                CategoryId = exist_cat.Id,
                                ProductId = product.Id
                            });
                            storeEntities.SaveChanges();
                        }
                        
                        ctxx.Send(increaseProductProgressValues, p);

                    }
                    catch (Exception ex)
                    {
                        EventLog.WriteEntry(this.ToString(), ex.ToString());
                        //if (num2 < 5)
                        //{
                        //ctxx.Send(writeToProductProgress, string.Format("Error trying to insert data; Try {0} of 5", (object)(num2 + 1)));
                        ctxx.Send(writeToProductProgress, "Error details: " + ex.Message);
                            ctxx.Send(writeToProductProgress, "Error details: " + ex.StackTrace);
                        //    ++num2;
                        //}
                        //else
                        //    goto label_37;
                    }
                }
                ctxx.Send(writeToProductProgress, $"Prducto y Categorias Sincronizados!");
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(this.ToString(), ex.ToString());
                ctxx.Send(writeToProductProgress, "Error details: " + ex.Message);
                ctxx.Send(writeToProductProgress, "Error details: " + ex.StackTrace);
            }


        }

        void InitSyncOrders(object ctx)
        {
            SynchronizationContext ct = (SynchronizationContext)ctx;

            try
            {
                kamscrin_npcommerceEntities storeEntities = new kamscrin_npcommerceEntities();
                ct.Send(writeToOrderProgress, string.Format("Obteniendo Ordenes desde la tienda"));
                //DateTime date = DateTime.UtcNow.AddMinutes(-5);
                //List<Order> list = storeEntities.Orders.Where<Order>(x => x.CreatedOnUtc >= date).ToList<Order>();

                var ordersIds = SQLHelper.getPCGRAFOrdes();

                List<Order> list = storeEntities.Orders.Where(x => !ordersIds.Contains(x.Id.ToString())).ToList();

                ct.Send(setOrderProgressInitialValues, list.Count);
                string str1 = "<Value>";
                string str2 = "</Value>";
                Console.WriteLine("Processing Orders");
                ct.Send(writeToOrderProgress, string.Format("Total Orders = {0}", (object)list.Count<Order>()));
                ct.Send(writeToOrderProgress, "");
                foreach (Order order in list)
                {
                    Order o = order;
                    try
                    {
                        Console.WriteLine(string.Format("Order ID = {0}", (object)o.Id));
                        Customer c = o.Customer;
                        GenericAttribute genericAttribute = ((IQueryable<GenericAttribute>)storeEntities.GenericAttributes).Where<GenericAttribute>(x => x.EntityId == c.Id && x.Key == "CustomCustomerAttributes" && x.KeyGroup == "Customer").First<GenericAttribute>();
                        string clientCode = genericAttribute == null || genericAttribute.Value.IndexOf(str2) <= 0 ? "'0000'" : "'" + genericAttribute.Value.Substring(0, genericAttribute.Value.IndexOf(str2)).Substring(genericAttribute.Value.IndexOf(str1) + str1.Length) + "'";

                        if (!Debugger.IsAttached)
                            SQLHelper.InsertOrder(o, clientCode);

                        DbSet<OrderItem> orderItems = storeEntities.OrderItems;
                        foreach (OrderItem orderitem in ((IQueryable<OrderItem>)orderItems).Where<OrderItem>(x => x.OrderId == o.Id).ToList<OrderItem>())
                        {
                            Console.WriteLine(string.Format("Item {0}", (object)orderitem.Id));

                            if (!Debugger.IsAttached)
                                SQLHelper.InsertOrderItems(o, orderitem);
                        }
                    }
                    catch (Exception ex)
                    {
                        EventLog.WriteEntry(this.ToString(), ex.ToString());
                        ct.Send(writeToOrderProgress, $"Error al Insertar la Orden: {o.OrderGuid} - {ex.Message}");
                        //Console.WriteLine("Error " + ex.Message);
                        //Console.WriteLine("Continue with next order");
                    }
                    ct.Send(increaseOrderProgressValues, o);
                    ct.Send(writeToOrderProgress, $"Se Inserto la Orden # {o.OrderGuid}");
                    Thread.Sleep(500);

                }
                Console.WriteLine("Orders Syncronize");
                Thread.Sleep(6000);
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine(ex.Message);
                sb.AppendLine("");
                sb.AppendLine(ex.StackTrace);
                sb.AppendLine("");
                EventLog.WriteEntry(this.ToString(), sb.ToString());
                ct.Send(writeToOrderProgress, $"Error en la sincronizacion: {sb.ToString()}");
            }
        }

        private static string RemoveSpecialCharacters(string str)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (char ch in str)
            {
                if (ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'Z' || (ch >= 'a' && ch <= 'z' || ch == '.') || ch == '_')
                    stringBuilder.Append(ch);
            }
            return stringBuilder.ToString();
        }

        private void btn_forceExecutionOrders_Click(object sender, RoutedEventArgs e)
        {
            var ctx = SynchronizationContext.Current;
            Thread th = new Thread(InitSyncOrders);
            txt_orders_log.Text = string.Empty;
            th.Start(ctx);
        }

        private void btn_forceExecutionProducts_Click(object sender, RoutedEventArgs e)
        {
            var ctx = SynchronizationContext.Current;
            Thread th = new Thread(InitSyncProducts);
            txt_orders_log.Text = string.Empty;
            th.Start(ctx);

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            keepExecutionOrders = false;
            keepExecutionProducts = false;
            if (OrdersTh != null && OrdersTh.IsAlive)
            {
                OrdersTh.Abort();
            }
            else {
                OrdersTh.Interrupt();
            }
            if (ProductsTh != null && ProductsTh.IsAlive) {
                ProductsTh.Abort();
            }
            else
            {
                ProductsTh.Interrupt();
            }
        }
    }
}
