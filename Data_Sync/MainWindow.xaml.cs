using Data_Sync.Data;
using Data_Sync.DTOs;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Data_Sync
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool keepExecutionOrders = true;
        private bool keepExecutionInventory = true;
        private bool keepExecutionProducts = true;
        private bool processingInventory;
        private TimeSpan orderDelay = TimeSpan.FromMinutes(2.0);
        private TimeSpan inventoryDelay = TimeSpan.FromMinutes(2.0);
        private TimeSpan ProductDelay = TimeSpan.FromDays(1.0);
        private Thread OrdersTh;
        private Thread ProductsTh;
        private Thread InventoryTh;
        private int Max_Chars = 15000;

        public MainWindow()
        {
            InitializeComponent(); SynchronizationContext current = SynchronizationContext.Current;
            ThreadContext parameter1 = new ThreadContext();
            parameter1.CT = current;
            this.OrdersTh = new Thread((ParameterizedThreadStart)(value =>
            {
                SynchronizationContext synchronizationContext = (SynchronizationContext)value;
                while (this.keepExecutionOrders)
                {
                    Thread.Sleep(this.orderDelay);
                    Thread thread = new Thread(new ParameterizedThreadStart(this.InitSyncOrders));
                    synchronizationContext.Send(new SendOrPostCallback(this.clearOrderLogText), (object)string.Empty);
                    SynchronizationContext parameter2 = synchronizationContext;
                    thread.Start((object)parameter2);
                }
            }));
            this.OrdersTh.Start((object)current);
            this.InventoryTh = new Thread((ParameterizedThreadStart)(value =>
            {
                ThreadContext threadContext = (ThreadContext)value;
                while (this.keepExecutionInventory)
                {
                    Thread.Sleep(this.inventoryDelay);
                    if (!this.processingInventory)
                    {
                        threadContext.CT.Send(new SendOrPostCallback(this.setInventoryProgressInitialValues), (object)0);
                        Thread thread = new Thread(new ParameterizedThreadStart(this.InitSyncInventoryMoves));
                        threadContext.CT.Send(new SendOrPostCallback(this.clearInventoryLogText), (object)string.Empty);
                        ThreadContext parameter3 = threadContext;
                        thread.Start((object)parameter3);
                    }
                }
            }));
            parameter1.Moves = SQLHelper.GetInventoryMoves();
            this.setInventoryProgressInitialValues((object)parameter1.Moves.Count);
            this.InventoryTh.Start((object)parameter1);
            this.ProductsTh = new Thread((ParameterizedThreadStart)(value =>
            {
                SynchronizationContext synchronizationContext = (SynchronizationContext)value;
                while (this.keepExecutionProducts)
                {
                    Thread.Sleep(this.ProductDelay);
                    Thread thread = new Thread(new ParameterizedThreadStart(this.InitSyncProducts));
                    synchronizationContext.Send(new SendOrPostCallback(this.clearProductLogText), (object)string.Empty);
                    SynchronizationContext parameter4 = synchronizationContext;
                    thread.Start((object)parameter4);
                }
            }));
            this.ProductsTh.Start((object)current);
        }

        private void btn_Orders_Click(object sender, RoutedEventArgs e)
        {
            SynchronizationContext current = SynchronizationContext.Current;
            Thread thread = new Thread(new ParameterizedThreadStart(this.InitSyncOrders));
            thread.Start(current);

        }

        private void btn_Products_Click(object sender, RoutedEventArgs e)
        {
            SynchronizationContext current = SynchronizationContext.Current;
            Thread thread = new Thread(new ParameterizedThreadStart(this.InitSyncProducts));
            thread.Start(current);
        }

        private void btn_Inventory_Click(object sender, RoutedEventArgs e)
        {
            if (this.processingInventory)
                return;
            this.setInventoryProgressInitialValues(0);
            List<InventoryMoves> source = new List<InventoryMoves>();
            int from = 0;
            int to = 1000;
            int num = 1000;
            while (source != null)
            {
                source = SQLHelper.GetInventoryMovesForced(from, to);
                if (source.Count > 0)
                {
                    ThreadContext threadContext = new ThreadContext();
                    threadContext.CT = SynchronizationContext.Current;
                    threadContext.Moves = source;
                    this.setIncrementInventoryProgressInitialValues(source.Count<InventoryMoves>());
                    Thread thread = new Thread(new ParameterizedThreadStart(this.InitSyncInventoryMoves));
                    this.txt_Inventory.Text = string.Empty;
                    ThreadContext parameter = threadContext;
                    thread.Start(parameter);
                    from += num;
                    to += num;
                }
                else
                    source = (List<InventoryMoves>)null;
            }

        }

        private void clearOrderLogText(object value)
        {
            txt_Orders.Text = string.Empty;
        }

        private void clearProductLogText(object value) => txt_Products.Text = string.Empty;

        private void clearInventoryLogText(object value) => txt_Inventory.Text = string.Empty;

        private void setOrderProgressInitialValues(object maxValue)
        {
            int num = (int)maxValue;
            this.pgr_Orders.Maximum = (double)num;
            this.pgr_Orders.Minimum = 0.0;
            this.pgr_Orders.Value = 0.0;
            //this.pgr_Orders. = string.Format("0 de {0}", num);
        }

        private void setProductProgressInitialValues(object maxValue)
        {
            int num = (int)maxValue;
            this.pgr_Products.Maximum = (double)num;
            this.pgr_Products.Minimum = 0.0;
            this.pgr_Products.Value = 0.0;
            //this.lbl_progressProducts.Content = string.Format("0 de {0}", num);
        }

        private void setInventoryProgressInitialValues(object maxValue)
        {
            int num = (int)maxValue;
            this.pgr_Inventory.Maximum = (double)num;
            this.pgr_Inventory.Minimum = 0.0;
            this.pgr_Inventory.Value = 0.0;
            //this.lbl_progressInventory.Content = string.Format("0 de {0}", num);
        }

        private void setIncrementInventoryProgressInitialValues(object maxValue)
        {
            this.pgr_Inventory.Maximum += (double)(int)maxValue;
            this.pgr_Inventory.Minimum = 0.0;
            //this.lbl_progressInventory.Content = string.Format("0 de {0}", this.prg_ProgressInventory.Maximum);
        }

        private void increaseOrderProgressValues(object maxValue)
        {
            this.pgr_Orders.Value += 1;
            //this.lbl_progressOrders.Content = string.Format("{0} de {1}", this.prg_ProgressOrders.Value, this.prg_ProgressOrders.Maximum);
        }

        private void increaseProductProgressValues(object maxValue)
        {
            this.pgr_Products.Value += 1;
            //this.lbl_progressProducts.Content = string.Format("{0} de {1}", this.prg_ProgressProducts.Value, this.prg_ProgressProducts.Maximum);
        }

        private void increaseInventoryProgressValues(object maxValue)
        {
            this.pgr_Inventory.Value += 1;
            //this.lbl_progressInventory.Content = string.Format("{0} de {1}", this.prg_ProgressInventory.Value, this.prg_ProgressInventory.Maximum);
        }

        private void writeToOrderProgress(object log)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(this.txt_Orders.Text.Length > Max_Chars ? string.Empty : this.txt_Orders.Text);
            stringBuilder.AppendLine((string)log);
            this.txt_Orders.Text = stringBuilder.ToString();
            this.svg_Orders.ScrollToEnd();
            LogHelper.WriteToLog((string)log);
        }

        private void writeToInventoryProgress(object log)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(this.txt_Inventory.Text.Length > Max_Chars ? string.Empty : this.txt_Inventory.Text);
            stringBuilder.AppendLine((string)log);
            this.txt_Inventory.Text = stringBuilder.ToString();
            this.svg_Inventory.ScrollToEnd();
            LogHelper.WriteToLog((string)log);
        }

        private void writeToErrorLog(object log) => LogHelper.WriteToErrorLog((string)log);

        private void writeToProductProgress(object log)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(this.txt_Products.Text.Length > Max_Chars ? string.Empty : this.txt_Products.Text);
            stringBuilder.AppendLine((string)log);
            this.txt_Products.Text = stringBuilder.ToString();
            this.svg_Products.ScrollToEnd();
            LogHelper.WriteToLog((string)log);
        }

        private void InitSyncProducts(object ctx)
        {
            SynchronizationContext synchronizationContext = (SynchronizationContext)ctx;
            try
            {
                kams_nopcEntities npcommerceEntities = new kams_nopcEntities();
                synchronizationContext.Send(new SendOrPostCallback(this.writeToProductProgress), "Opteniendo Lista de Grupos");
                List<Group> categories = SQLHelper.getCategories();
                int num = 0;
                synchronizationContext.Send(new SendOrPostCallback(this.writeToProductProgress), string.Format("Procesando {0} Categorias", categories.Count));
                synchronizationContext.Send(new SendOrPostCallback(this.setProductProgressInitialValues), categories.Count);
                foreach (Group state in categories)
                {
                    Group g = state;
                    synchronizationContext.Send(new SendOrPostCallback(this.writeToProductProgress), ("Categoria -> " + g.SDescripcion + " - " + g.SGrupo));
                    Console.WriteLine();
                    Category ct = new Category()
                    {
                        Name = g.SDescripcion,
                        Description = g.SDescripcion,
                        MetaDescription = g.SGrupo,
                        MetaKeywords = g.SGrupo_Padre,
                        PageSize = 10
                    };
                    Category category = ((IQueryable<Category>)npcommerceEntities.Categories).FirstOrDefault<Category>((Expression<Func<Category, bool>>)(x => x.MetaDescription == g.SGrupo_Padre));
                    Category exist = ((IQueryable<Category>)npcommerceEntities.Categories).FirstOrDefault<Category>((Expression<Func<Category, bool>>)(x => x.MetaDescription == g.SGrupo));
                    if (exist == null)
                    {
                        if (category != null)
                            num = category.Id;
                        ct.ParentCategoryId = num;
                        npcommerceEntities.Categories.Add(ct);
                        npcommerceEntities.SaveChanges();
                        if (((IQueryable<UrlRecord>)npcommerceEntities.UrlRecords).FirstOrDefault<UrlRecord>((Expression<Func<UrlRecord, bool>>)(x => x.EntityId == ct.Id && x.EntityName == "Category")) == null)
                        {
                            npcommerceEntities.UrlRecords.Add(new UrlRecord()
                            {
                                EntityId = ct.Id,
                                EntityName = "Category",
                                Slug = MainWindow.RemoveSpecialCharacters(ct.Name),
                                IsActive = true
                            });
                            npcommerceEntities.SaveChanges();
                        }
                    }
                    else
                    {
                        exist.Description = g.SDescripcion;
                        exist.Name = g.SDescripcion;
                        exist.MetaDescription = g.SGrupo;
                        exist.MetaKeywords = g.SGrupo_Padre;
                        exist.PageSize = 10;
                        npcommerceEntities.SaveChanges();
                        if (((IQueryable<UrlRecord>)npcommerceEntities.UrlRecords).FirstOrDefault<UrlRecord>((Expression<Func<UrlRecord, bool>>)(x => x.EntityId == exist.Id && x.EntityName == "Category")) == null)
                        {
                            npcommerceEntities.UrlRecords.Add(new UrlRecord()
                            {
                                EntityId = exist.Id,
                                EntityName = "Category",
                                Slug = MainWindow.RemoveSpecialCharacters(exist.Name),
                                IsActive = true
                            });
                            npcommerceEntities.SaveChanges();
                        }
                    }
                    synchronizationContext.Send(new SendOrPostCallback(this.increaseProductProgressValues), state);
                }
                synchronizationContext.Send(new SendOrPostCallback(this.writeToProductProgress), "Obteniendo Prductos");
                List<PCGRAF_Product> products = SQLHelper.getProducts();
                synchronizationContext.Send(new SendOrPostCallback(this.writeToProductProgress), string.Format("Procesando {0} Productos", products.Count));
                synchronizationContext.Send(new SendOrPostCallback(this.setProductProgressInitialValues), products.Count);
                foreach (PCGRAF_Product pcgrafProduct in products)
                {
                    PCGRAF_Product p = pcgrafProduct;
                    synchronizationContext.Send(new SendOrPostCallback(this.writeToProductProgress), ("Prducto " + p.SCodigo_Producto + " - P1: " + p.P1 + " - P2: " + p.P2 + " - P3: " + p.P3 + " - P4: " + p.P4));
                    try
                    {
                        decimal result1 = 0;
                        decimal result2 = 0;
                        decimal result3 = 0;
                        decimal result4 = 0;
                        if (p.P1 != null)
                            Decimal.TryParse(p.P1, out result1);
                        if (p.P2 != null)
                            Decimal.TryParse(p.P2, out result2);
                        if (p.P3 != null)
                            Decimal.TryParse(p.P3, out result3);
                        if (p.P4 != null)
                            Decimal.TryParse(p.P4, out result4);
                        Category exist_cat = ((IQueryable<Category>)npcommerceEntities.Categories).FirstOrDefault<Category>((Expression<Func<Category, bool>>)(x => x.MetaDescription == p.SGrupo));
                        Product product = new Product()
                        {
                            Name = p.SDescripcion_Inventario,
                            ShortDescription = p.SDescripcion_Inventario,
                            Price1 = result1,
                            Price2 = result2,
                            Price3 = result3,
                            Price4 = result4,
                            MetaKeywords = p.SCodigo_Producto,
                            Sku = p.SCodigo_Producto,
                            ProductTypeId = 5,
                            CreatedOnUtc = DateTime.UtcNow,
                            StockQuantity = 9999,
                            VisibleIndividually = true,
                            OrderMaximumQuantity = 999,
                            OrderMinimumQuantity = 1,
                            Published = false
                        };
                        Product exist_prod = ((IQueryable<Product>)npcommerceEntities.Products).FirstOrDefault<Product>((Expression<Func<Product, bool>>)(x => x.MetaKeywords == p.SCodigo_Producto));
                        if (exist_prod != null)
                        {
                            Product_Category_Mapping productCategoryMapping = ((IQueryable<Product_Category_Mapping>)npcommerceEntities.Product_Category_Mapping).FirstOrDefault<Product_Category_Mapping>((Expression<Func<Product_Category_Mapping, bool>>)(x => x.CategoryId == exist_cat.Id && x.ProductId == exist_prod.Id));
                            if (productCategoryMapping == null && exist_prod != null)
                            {
                                npcommerceEntities.Product_Category_Mapping.Add(new Product_Category_Mapping()
                                {
                                    CategoryId = exist_cat.Id,
                                    ProductId = exist_prod.Id
                                });
                                npcommerceEntities.SaveChanges();
                            }
                            else if (productCategoryMapping != null && exist_prod != null)
                            {
                                productCategoryMapping.ProductId = exist_prod.Id;
                                productCategoryMapping.CategoryId = exist_cat.Id;
                                exist_prod.Price1 = (Decimal.Parse(p.P1));
                                exist_prod.Price2 = (Decimal.Parse(p.P2));
                                exist_prod.Price3 = (Decimal.Parse(p.P3));
                                exist_prod.Price4 = (Decimal.Parse(p.P4));
                                exist_prod.Name = p.SDescripcion_Inventario;
                                exist_prod.ShortDescription = p.SDescripcion_Inventario;
                                exist_prod.MetaKeywords = p.SCodigo_Producto;
                                exist_prod.OrderMinimumQuantity = 1;
                                exist_prod.OrderMaximumQuantity = 999;
                                exist_prod.ProductTypeId = 5;
                                exist_prod.Published = exist_prod.Published;
                                npcommerceEntities.SaveChanges();
                            }
                            else
                            {
                                npcommerceEntities.Products.Add(product);
                                npcommerceEntities.SaveChanges();
                                Thread.Sleep(500);
                                npcommerceEntities.Product_Category_Mapping.Add(new Product_Category_Mapping()
                                {
                                    CategoryId = exist_cat.Id,
                                    ProductId = product.Id
                                });
                                npcommerceEntities.SaveChanges();
                            }
                            if (((IQueryable<UrlRecord>)npcommerceEntities.UrlRecords).FirstOrDefault<UrlRecord>((Expression<Func<UrlRecord, bool>>)(x => x.EntityId == exist_prod.Id && x.EntityName == "Product")) == null)
                            {
                                npcommerceEntities.UrlRecords.Add(new UrlRecord()
                                {
                                    EntityId = exist_prod.Id,
                                    EntityName = "Product",
                                    Slug = MainWindow.RemoveSpecialCharacters(exist_prod.Name),
                                    IsActive = true
                                });
                                npcommerceEntities.SaveChanges();
                            }
                        }
                        else
                        {
                            npcommerceEntities.Products.Add(product);
                            npcommerceEntities.SaveChanges();
                            npcommerceEntities.UrlRecords.Add(new UrlRecord()
                            {
                                EntityId = product.Id,
                                EntityName = "Product",
                                Slug = MainWindow.RemoveSpecialCharacters(p.SDescripcion_Inventario),
                                IsActive = true
                            });
                            npcommerceEntities.Product_Category_Mapping.Add(new Product_Category_Mapping()
                            {
                                CategoryId = exist_cat.Id,
                                ProductId = product.Id
                            });
                            npcommerceEntities.SaveChanges();
                        }
                        synchronizationContext.Send(new SendOrPostCallback(this.increaseProductProgressValues), p);
                    }
                    catch (Exception ex)
                    {
                        synchronizationContext.Send(new SendOrPostCallback(this.writeToErrorLog), ("Error details: " + ex.Message));
                        synchronizationContext.Send(new SendOrPostCallback(this.writeToErrorLog), ("Error details: " + ex.StackTrace));
                    }
                }
                synchronizationContext.Send(new SendOrPostCallback(this.writeToProductProgress), "Prducto y Categorias Sincronizados!");
            }
            catch (Exception ex)
            {
                synchronizationContext.Send(new SendOrPostCallback(this.writeToErrorLog), ("Error details: " + ex.Message));
                synchronizationContext.Send(new SendOrPostCallback(this.writeToErrorLog), ("Error details: " + ex.StackTrace));
            }
        }


        private async void InitSyncInventoryMoves(object ctx)
        {
            MainWindow mainWindow = this;
            ThreadContext ct = (ThreadContext)null;
            try
            {
                ct = (ThreadContext)ctx;
                mainWindow.processingInventory = true;
                kams_nopcEntities storeEntities = new kams_nopcEntities();
                ct.CT.Send(new SendOrPostCallback(mainWindow.writeToInventoryProgress), string.Format("Obteniendo Movimeitos de Inventario"));
                ct.CT.Send(new SendOrPostCallback(mainWindow.writeToInventoryProgress), string.Format("***********************************************"));
                List<string> codes = ct.Moves.Select<InventoryMoves, string>((Func<InventoryMoves, string>)(y => y.CodArticulo)).ToList<string>();
                ct.CT.Send(new SendOrPostCallback(mainWindow.writeToInventoryProgress), string.Format("Obteniendo Productos relacionados"));
                List<Product> products = await QueryableExtensions.ToListAsync<Product>(((IQueryable<Product>)storeEntities.Products).Where<Product>((Expression<Func<Product, bool>>)(x => codes.Contains(x.Sku))));
                ct.CT.Send(new SendOrPostCallback(mainWindow.writeToInventoryProgress), string.Format(string.Format("Movimientos {0} Productos", ct.Moves.Count)));
                foreach (InventoryMoves move1 in ct.Moves)
                {
                    InventoryMoves move = move1;
                    Product prod = products.FirstOrDefault<Product>((Func<Product, bool>)(x => x.Sku == move.CodArticulo));
                    await mainWindow.ProcessInventoryMove(move, prod, ct.CT, storeEntities);
                }
                ct.CT.Send(new SendOrPostCallback(mainWindow.writeToErrorLog), string.Format(string.Format("Se Procesaron {0} movimientos --- {1}", ct.Moves.Count, DateTime.Now)));
                ct.CT.Send(new SendOrPostCallback(mainWindow.writeToInventoryProgress), "Inventario Sincronizada");
                mainWindow.processingInventory = false;
                storeEntities = (kams_nopcEntities)null;
                products = (List<Product>)null;
                ct = (ThreadContext)null;
            }
            catch (Exception ex)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine(ex.Message);
                stringBuilder.AppendLine("");
                stringBuilder.AppendLine(ex.StackTrace);
                stringBuilder.AppendLine("");
                if (ct.CT == null)
                {
                    ct = (ThreadContext)null;
                }
                else
                {
                    ct.CT.Send(new SendOrPostCallback(mainWindow.writeToErrorLog), ("Error en la sincronizacion: " + stringBuilder.ToString()));
                    ct = (ThreadContext)null;
                }
            }
        }


        private async Task ProcessInventoryMove(
          InventoryMoves move,
          Product prod,
          SynchronizationContext ct,
          kams_nopcEntities storeEntities)
        {
            MainWindow mainWindow = this;
            try
            {
                if (prod == null)
                    return;
                ct.Send(new SendOrPostCallback(mainWindow.writeToInventoryProgress), string.Format(string.Format("Procesando Producto {0} - Codigo {1} Cantidad {2}", prod.FullDescription, prod.Sku, move.Cantidad)));
                prod.StockQuantity = Convert.ToInt32(move.Cantidad);
                storeEntities.Entry<Product>(prod).State = (EntityState)16;
                int num = await storeEntities.SaveChangesAsync();
                ct.Send(new SendOrPostCallback(mainWindow.increaseInventoryProgressValues), 0);
            }
            catch (Exception ex)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine(ex.Message);
                stringBuilder.AppendLine("");
                stringBuilder.AppendLine(ex.StackTrace);
                stringBuilder.AppendLine("");
                ct?.Send(new SendOrPostCallback(mainWindow.writeToErrorLog), ("Error en la sincronizacion: " + stringBuilder.ToString()));
            }
        }

        private void InitSyncOrders(object ctx)
        {
            SynchronizationContext synchronizationContext = (SynchronizationContext)ctx;
            try
            {
                // ISSUE: object of a compiler-generated type is created
                // ISSUE: variable of a compiler-generated type                  
                kams_nopcEntities npcommerceEntities = new kams_nopcEntities();
                synchronizationContext.Send(new SendOrPostCallback(this.writeToOrderProgress), string.Format("Obteniendo Ordenes desde la tienda"));
                // ISSUE: reference to a compiler-generated field
                var ordersIds = SQLHelper.getPCGRAFOrdes();
                ParameterExpression parameterExpression;
                // ISSUE: reference to a compiler-generated field
                // ISSUE: method reference
                // ISSUE: type reference
                // ISSUE: method reference
                List<Order> list = npcommerceEntities.Orders.Include(x=> x.Customer).Include(x => x.OrderItems).Where(x => !ordersIds.Contains(x.Id.ToString())).ToList();
                
                synchronizationContext.Send(new SendOrPostCallback(this.setOrderProgressInitialValues), list.Count);
                string str1 = "<Value>";
                string str2 = "</Value>";
                synchronizationContext.Send(new SendOrPostCallback(this.writeToOrderProgress), "Processing Orders");
                synchronizationContext.Send(new SendOrPostCallback(this.writeToOrderProgress), string.Format("Total Orders = {0}", list.Count<Order>()));
                synchronizationContext.Send(new SendOrPostCallback(this.writeToOrderProgress), "");
                foreach (Order order in list)
                {
                    try
                    {
                        // ISSUE: reference to a compiler-generated field
                        synchronizationContext.Send(new SendOrPostCallback(this.writeToOrderProgress), string.Format("Order ID = {0}", order.Id));

                        GenericAttribute genericAttribute = ((IQueryable<GenericAttribute>)npcommerceEntities.GenericAttributes).Where<GenericAttribute>((Expression<Func<GenericAttribute, bool>>)(x => x.EntityId == order.Customer.Id && x.Key == "CustomCustomerAttributes" && x.KeyGroup == "Customer")).First<GenericAttribute>();
                        string clientCode = genericAttribute == null || genericAttribute.Value.IndexOf(str2) <= 0 ? "'0000'" : "'" + genericAttribute.Value.Substring(0, genericAttribute.Value.IndexOf(str2)).Substring(genericAttribute.Value.IndexOf(str1) + str1.Length) + "'";
                        if (!Debugger.IsAttached)
                        {
                            // ISSUE: reference to a compiler-generated field
                            SQLHelper.InsertOrder(order, clientCode);
                        }

                        foreach (OrderItem orderitem in order.OrderItems)
                        {
                            synchronizationContext.Send(new SendOrPostCallback(this.writeToOrderProgress), string.Format("Item {0}", orderitem.Id));
                            if (!Debugger.IsAttached)
                            {
                                // ISSUE: reference to a compiler-generated field
                                SQLHelper.InsertOrderItems(order, orderitem);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // ISSUE: reference to a compiler-generated field
                        synchronizationContext.Send(new SendOrPostCallback(this.writeToErrorLog), string.Format("Error al Insertar la Orden: {0} - {1}", order.Id, ex.Message));
                    }
                    // ISSUE: reference to a compiler-generated field
                    synchronizationContext.Send(new SendOrPostCallback(this.increaseOrderProgressValues), order);
                    // ISSUE: reference to a compiler-generated field
                    synchronizationContext.Send(new SendOrPostCallback(this.writeToOrderProgress), string.Format("Se Inserto la Orden # {0}",order.Id));
                    Thread.Sleep(500);
                }
                synchronizationContext.Send(new SendOrPostCallback(this.writeToOrderProgress), "Orders Syncronize");
                Thread.Sleep(3000);
            }
            catch (Exception ex)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine(ex.Message);
                stringBuilder.AppendLine("");
                stringBuilder.AppendLine(ex.StackTrace);
                stringBuilder.AppendLine("");
                synchronizationContext.Send(new SendOrPostCallback(this.writeToErrorLog), ("Error en la sincronizacion: " + stringBuilder.ToString()));
            }
        }

        private static string RemoveSpecialCharacters(string str)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (char ch in str)
            {
                if (ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'Z' || ch >= 'a' && ch <= 'z' || ch == '.' || ch == '_')
                    stringBuilder.Append(ch);
            }
            return stringBuilder.ToString();
        }
    }
    internal class ThreadContext
    {
        public SynchronizationContext CT { get; set; }

        public List<InventoryMoves> Moves { get; set; }
    }
}
