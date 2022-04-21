using Data_Sync.DTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Sync.Data
{
    public class SQLHelper
    {
        private const string SQL_CONNECTION_STRING = "data source=10.10.10.100; initial catalog = siawin0; persist security info=True; Integrated Security = SSPI;";
        private const string SQL_CONNECTION_STRING_INVENTORY = "data source=localhost; initial catalog = KamsOnline; persist security info=True; Integrated Security = SSPI;";

        public static List<Group> getCategories()
        {
            SqlConnection connection = new SqlConnection("data source=10.10.10.100; initial catalog = siawin0; persist security info=True; Integrated Security = SSPI;");
            List<Group> groupList = new List<Group>();
            SqlCommand sqlCommand = new SqlCommand("select sgrupo,sdescripcion,sgrupo_padre from in01", connection);
            connection.Open();
            SqlDataReader reader = sqlCommand.ExecuteReader();
            DataTable group = new DataTable();
            group.Load((IDataReader)reader);
            connection.Close();
            connection.Dispose();
            return Group.getListFromDataTable(group);
        }

        public static List<PCGRAF_Product> getProducts()
        {
            SqlConnection connection = new SqlConnection("data source=10.10.10.100; initial catalog = siawin0; persist security info=True; Integrated Security = SSPI;");
            List<Product> productList = new List<Product>();
            SqlCommand sqlCommand = new SqlCommand("With _sku as (select SKU from [WDB2.MY-HOSTING-PANEL.COM].[kamscrin_npcommerce].[kamscrin_admin].[Product] p \r\n                                where p.Id in (SELECT distinct [ProductId] FROM [WDB2.MY-HOSTING-PANEL.COM].[kamscrin_npcommerce].[kamscrin_admin].[Product_Picture_Mapping])),\r\n                                prices as (select ii10.sCodigo_Articulo as scodigo_producto, i04.sDescripcion_Inventario as sdescripcion_inventario, i04.sgrupo, ((i04.cPrecio_Publico * 0.13) + i04.cPrecio_Publico) as P1,\r\n                                (select i10.cPrecio_Publico from IN10 as i10 where i10.sCodigo_Lista = '002' and i10.sCodigo_Articulo = ii10.sCodigo_Articulo) as P2,\r\n                                (select i10.cPrecio_Publico from IN10 as i10 where i10.sCodigo_Lista = '003' and i10.sCodigo_Articulo = ii10.sCodigo_Articulo) as P3, \r\n                                (select i10.cPrecio_Publico from IN10 as i10 where i10.sCodigo_Lista = '004' and i10.sCodigo_Articulo = ii10.sCodigo_Articulo) as P4 \r\n                                from IN10 as ii10 inner join IN04 as i04 on ii10.sCodigo_Articulo = i04.sCodigo_Producto where ii10.sCodigo_Articulo in (select * from _sku))\r\n                                select * from prices as p group by p.scodigo_producto,p.sdescripcion_inventario,p.sGrupo, p.P1, p.P2, p.P3, p.P4", connection);
            connection.Open();
            SqlDataReader reader = sqlCommand.ExecuteReader();
            DataTable group = new DataTable();
            group.Load((IDataReader)reader);
            connection.Close();
            connection.Dispose();
            return PCGRAF_Product.getListFromDataTable(group);
        }

        public static List<PCGRAF_Product> getProductsPaged(int from, int to)
        {
            SqlConnection connection = new SqlConnection("data source=10.10.10.100; initial catalog = siawin0; persist security info=True; Integrated Security = SSPI;");
            List<Product> productList = new List<Product>();
            SqlCommand sqlCommand = new SqlCommand("With prices as (select ii10.sCodigo_Articulo as scodigo_producto,i04.sDescripcion_Inventario as sdescripcion_inventario, i04.sgrupo, i04.cPrecio_Publico as P1,(select i10.cPrecio_Publico from IN10 as i10 where i10.sCodigo_Lista = '002' and i10.sCodigo_Articulo = ii10.sCodigo_Articulo) as P2,(select i10.cPrecio_Publico from IN10 as i10 where i10.sCodigo_Lista = '003' and i10.sCodigo_Articulo = ii10.sCodigo_Articulo) as P3,(select i10.cPrecio_Publico from IN10 as i10 where i10.sCodigo_Lista = '004' and i10.sCodigo_Articulo = ii10.sCodigo_Articulo) as P4from IN10 as ii10 inner join IN04 as i04 on ii10.sCodigo_Articulo = i04.sCodigo_Producto)select ROW_NUMBER() OVER(ORDER BY scodigo_producto ASC) AS _Row,*from prices as p group by p.scodigo_producto,p.sdescripcion_inventario,p.sGrupo, p.P1, p.P2, p.P3, p.P4", connection);
            connection.Open();
            SqlDataReader reader = sqlCommand.ExecuteReader();
            DataTable group = new DataTable();
            group.Load((IDataReader)reader);
            connection.Close();
            connection.Dispose();
            return PCGRAF_Product.getListFromDataTable(group);
        }

        public static List<string> getPCGRAFOrdes()
        {
            SqlConnection connection = new SqlConnection("data source=10.10.10.100; initial catalog = siawin0; persist security info=True; Integrated Security = SSPI;");
            List<string> pcgrafOrdes = new List<string>();
            SqlCommand sqlCommand = new SqlCommand("select sPedido from RT00", connection);
            connection.Open();
            SqlDataReader reader = sqlCommand.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load((IDataReader)reader);
            foreach (DataRow row in (InternalDataCollectionBase)dataTable.Rows)
                pcgrafOrdes.Add(row[0].ToString());
            return pcgrafOrdes;
        }

        public static void InsertOrder(Order order, string clientCode)
        {
            int id = order.Id;
            LogHelper.WriteToLog(string.Format("Order ID {0}", (object)id));
            string cmdText = "insert into RT00(sPedido, dFecha, sCodigoCliente, sNombreCliente, sDireccion, sVendedor, sTransporte, sOrdenCompra,sNotasPedido, sNotasEntrega, sBodega, bTipoPrecio, bMoneda, cTipoCambio, cMontoTotal, bEstado, sQuien_Ingreso,dFecha_Ingreso, sHora_Ingreso, sQuien_Modifico, dFecha_Modifico, sHora_Modifico, cDesc1Maximo, cDesc2Maximo,cDesc1Pactado, cDesc2Pactado, cPorceExento, sListaPrecios, cTotalEmpaque1, bVisita, sProAtencion, sProCondiciones,sProVigencia, sCodFactura, sNotaCreditoCobro) VALUES(" + string.Format("{0}, GETDATE(),{1}, '{2} {3}',", (object)id, (object)clientCode, (object)order.Customer.Address.FirstName, (object)order.Customer.Address.LastName) + " '" + order.Address.Address1 + " " + order.Address.Address2 + ", " + order.Address.Country.Name + "','01', 'Transporte Kams', '', 'Pedido Tienda WEB', '" + order.Address.PhoneNumber + "'," + string.Format("'01', 1, 0, 586.21,{0} ,2,'WEB',getdate(),'',", (object)order.OrderTotal) + "'web',getdate(),'',0,0,0,0,0,'004',0,0,'','',0,'','')";
            SqlConnection connection = new SqlConnection("data source=10.10.10.100; initial catalog = siawin0; persist security info=True; Integrated Security = SSPI;");
            LogHelper.WriteToLog("SQL Command " + cmdText);
            SqlCommand sqlCommand = new SqlCommand(cmdText, connection);
            connection.Open();
            sqlCommand.ExecuteNonQuery();
            connection.Close();
        }

        public static void InsertOrderItems(Order order, OrderItem orderitem)
        {
            int id = order.Id;
            string str = orderitem.Product.FullDescription.Length > 100 ? orderitem.Product.FullDescription.Substring(0, 100) : orderitem.Product.FullDescription;
            string cmdText = "INSERT INTO RT01(sPedido, sCodigoProducto, sDescripcion, cCantidad, cPrecioVenta, cDescuento01, cDescuento02, sListaAplicada, cRegalia, cInventarioCliente,sQuien_Ingreso, dFecha_Ingreso, sHora_Ingreso, cImpuestoVentas, cDescuentoAnterior)" + string.Format("VALUES({0},'{1}','{2}',{3},{4},", (object)id, (object)orderitem.Product.Sku, (object)str, (object)orderitem.Quantity, (object)orderitem.PriceInclTax) + string.Format("{0},0,'004',0,0,'dsg',", (object)orderitem.DiscountAmountInclTax) + "getdate(),CONVERT(VARCHAR(10), CAST(getdate() AS TIME), 0),0.13,0)";
            SqlConnection connection = new SqlConnection("data source=10.10.10.100; initial catalog = siawin0; persist security info=True; Integrated Security = SSPI;");
            SqlCommand sqlCommand = new SqlCommand(cmdText, connection);
            LogHelper.WriteToLog("SQL Command " + cmdText);
            connection.Open();
            sqlCommand.ExecuteNonQuery();
            connection.Close();
        }

        public static List<InventoryMoves> GetInventoryMoves()
        {
            List<InventoryMoves> inventoryMovesList = new List<InventoryMoves>();
            SqlConnection connection = new SqlConnection("data source=localhost; initial catalog = KamsOnline; persist security info=True; Integrated Security = SSPI;");
            SqlCommand sqlCommand = new SqlCommand("select * from CatalogoArticulosNewNV where FechaActualizadoProducto >= @date", connection);
            sqlCommand.Parameters.Add("@date", SqlDbType.DateTime).Value = (object)DateTime.Now.AddMinutes(-2.0);
            connection.Open();
            SqlDataReader reader = sqlCommand.ExecuteReader();
            DataTable group = new DataTable();
            group.Load((IDataReader)reader);
            connection.Close();
            connection.Dispose();
            return InventoryMoves.getListFromDataTable(group);
        }

        public static List<InventoryMoves> GetInventoryMovesForced(int from, int to)
        {
            List<InventoryMoves> inventoryMovesList = new List<InventoryMoves>();
            SqlConnection connection = new SqlConnection("data source=localhost; initial catalog = KamsOnline; persist security info=True; Integrated Security = SSPI;");
            long num = to > 0 ? (long)to : long.MaxValue;
            SqlCommand sqlCommand = new SqlCommand(string.Format("with _rows as (select ROW_NUMBER() OVER(ORDER BY CodArticulo ASC) AS Row, * from CatalogoArticulosNewNV where Cantidad > 0) select * from _rows where (row > {0} and row <= {1})", (object)from, (object)num), connection);
            connection.Open();
            SqlDataReader reader = sqlCommand.ExecuteReader();
            DataTable group = new DataTable();
            group.Load((IDataReader)reader);
            connection.Close();
            connection.Dispose();
            return InventoryMoves.getListFromDataTable(group);
        }
    }
}
