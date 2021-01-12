using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary
{
    public class SQLHelper
    {
        private const string SQL_CONNECTION_STRING = "data source=10.10.10.100; initial catalog = siawin0; persist security info=True; Integrated Security = SSPI;";

        public static List<Group> getCategories()
        {
            SqlConnection connection = new SqlConnection(SQL_CONNECTION_STRING);
            List<Group> groupList = new List<Group>();
            SqlCommand sqlCommand = new SqlCommand("select sgrupo,sdescripcion,sgrupo_padre from in01", connection);
            connection.Open();
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            DataTable group = new DataTable();
            group.Load((IDataReader)sqlDataReader);
            connection.Close();
            return Group.getListFromDataTable(group);
        }

        public static List<PCGRAF_Product> getProducts()
        {
            SqlConnection connection = new SqlConnection(SQL_CONNECTION_STRING);
            List<Product> productList = new List<Product>();
            SqlCommand sqlCommand = new SqlCommand(@"With prices as (select ii10.sCodigo_Articulo as scodigo_producto, i04.sDescripcion_Inventario as sdescripcion_inventario, i04.sgrupo, i04.cPrecio_Publico as P1,
                (select i10.cPrecio_Publico from IN10 as i10 where i10.sCodigo_Lista = '002' and i10.sCodigo_Articulo = ii10.sCodigo_Articulo) as P2,
                (select i10.cPrecio_Publico from IN10 as i10 where i10.sCodigo_Lista = '003' and i10.sCodigo_Articulo = ii10.sCodigo_Articulo) as P3, 
                (select i10.cPrecio_Publico from IN10 as i10 where i10.sCodigo_Lista = '004' and i10.sCodigo_Articulo = ii10.sCodigo_Articulo) as P4
                from IN10 as ii10 inner join IN04 as i04 on ii10.sCodigo_Articulo = i04.sCodigo_Producto)
                select* from prices as p group by p.scodigo_producto,p.sdescripcion_inventario,p.sGrupo, p.P1, p.P2, p.P3, p.P4", connection);
            connection.Open();
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            DataTable group = new DataTable();
            group.Load((IDataReader)sqlDataReader);
            connection.Close();
            return PCGRAF_Product.getListFromDataTable(group);
        }

        public static List<string> getPCGRAFOrdes() {

            SqlConnection connection = new SqlConnection(SQL_CONNECTION_STRING);
            List<string> ordersId = new List<string>();
            SqlCommand sqlCommand = new SqlCommand("select sPedido from RT00", connection);
            connection.Open();
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            DataTable ids = new DataTable();
            ids.Load((IDataReader)sqlDataReader);

            foreach (DataRow r in ids.Rows) {
                ordersId.Add(r[0].ToString());
            }

            return ordersId;
        }

        public static void InsertOrder(Order order, string clientCode)
        {
            var orderID =  order.Id;
            LogHelper.WriteToLog($"Order ID {orderID}");
            string cmdText = "insert into RT00(sPedido, dFecha, sCodigoCliente, sNombreCliente, sDireccion, sVendedor, sTransporte, sOrdenCompra,sNotasPedido, sNotasEntrega, sBodega, bTipoPrecio, bMoneda, cTipoCambio, cMontoTotal, bEstado, sQuien_Ingreso,dFecha_Ingreso, sHora_Ingreso, sQuien_Modifico, dFecha_Modifico, sHora_Modifico, cDesc1Maximo, cDesc2Maximo,cDesc1Pactado, cDesc2Pactado, cPorceExento, sListaPrecios, cTotalEmpaque1, bVisita, sProAtencion, sProCondiciones,sProVigencia, sCodFactura, sNotaCreditoCobro) " +
                "VALUES(" + string.Format("{0}, GETDATE(),{1}, '{2} {3}',", orderID, clientCode, order.Customer.Address.FirstName, order.Customer.Address.LastName) + " '" + order.Address.Address1 + " " + order.Address.Address2 + ", " + order.Address.Country.Name + "','01', 'Transporte Kams', '', 'Pedido Tienda WEB', '" + order.Address.PhoneNumber + "'," + string.Format("'01', 1, 0, 586.21,{0} ,2,'WEB',getdate(),'',", order.OrderTotal) + "'web',getdate(),'',0,0,0,0,0,'004',0,0,'','',0,'','')";
            SqlConnection connection = new SqlConnection(SQL_CONNECTION_STRING);
            LogHelper.WriteToLog($"SQL Command {cmdText}");
            SqlCommand sqlCommand = new SqlCommand(cmdText, connection);
            connection.Open();
            sqlCommand.ExecuteNonQuery();
            connection.Close();
        }

        public static void InsertOrderItems(Order order, OrderItem orderitem)
        {
            var orderID = order.Id;
            string str = orderitem.Product.FullDescription.Length > 100 ? orderitem.Product.FullDescription.Substring(0, 100) : orderitem.Product.FullDescription;
            string cmdText = "INSERT INTO RT01(sPedido, sCodigoProducto, sDescripcion, cCantidad, cPrecioVenta, cDescuento01, cDescuento02, sListaAplicada, cRegalia, cInventarioCliente,sQuien_Ingreso, dFecha_Ingreso, sHora_Ingreso, cImpuestoVentas, cDescuentoAnterior)" + string.Format("VALUES({0},{1},'{2}',{3},{4},", orderID, orderitem.Product.Sku, str, orderitem.Quantity, orderitem.PriceInclTax) + string.Format("{0},0,'004',0,0,'dsg',", orderitem.DiscountAmountInclTax) + "getdate(),CONVERT(VARCHAR(10), CAST(getdate() AS TIME), 0),0.13,0)";
            SqlConnection connection = new SqlConnection(SQL_CONNECTION_STRING);
            SqlCommand sqlCommand = new SqlCommand(cmdText, connection); 
            LogHelper.WriteToLog($"SQL Command {cmdText}");
            connection.Open();
            sqlCommand.ExecuteNonQuery();
            connection.Close();
        }
    }
}
