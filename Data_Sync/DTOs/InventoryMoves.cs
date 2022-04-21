using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Data_Sync.DTOs
{

    public class InventoryMoves
    {
        public long Id { get; set; }

        public string CodArticulo { get; set; }

        public string CodProveedor { get; set; }

        public string CategoriaDefecto { get; set; }

        public string NombreArticulo { get; set; }

        public string EspecificacionArticulo { get; set; }

        public string Marca { get; set; }

        public double Cantidad { get; set; }

        public double PesoArticulo { get; set; }

        public double Alto { get; set; }

        public double Ancho { get; set; }

        public double Profundidad { get; set; }

        public double ImpuestoArticulo { get; set; }

        public double Costo_local { get; set; }

        public int Estado { get; set; }

        public DateTime FechaAgregado { get; set; }

        public int Insertado { get; set; }

        public int VisibleTienda { get; set; }

        public double PrecioCosto { get; set; }

        public string MetaTitulo { get; set; }

        public string MetaDescripcion { get; set; }

        public string Etiquetas { get; set; }

        public string CodigoBarras { get; set; }

        public string CodigoISBN { get; set; }

        public string CodigoEAN13 { get; set; }

        public string CondicionProducto { get; set; }

        public bool DisponibleConsulta { get; set; }

        public DateTime FechaDisponibilidad { get; set; }

        public DateTime FechaCreadoProducto { get; set; }

        public DateTime FechaActualizadoProducto { get; set; }

        public bool MostrarCondicion { get; set; }

        public int Insertado2 { get; set; }

        public static List<InventoryMoves> getListFromDataTable(DataTable group)
        {
            List<InventoryMoves> listFromDataTable = new List<InventoryMoves>();
            PropertyInfo[] properties = typeof(InventoryMoves).GetProperties();
            try
            {
                foreach (DataRow row in (InternalDataCollectionBase)group.Rows)
                {
                    InventoryMoves inventoryMoves = new InventoryMoves();
                    foreach (PropertyInfo propertyInfo in properties)
                    {
                        if (!string.IsNullOrEmpty(row[propertyInfo.Name.ToLower()].ToString()))
                            propertyInfo.SetValue((object)inventoryMoves, Convert.ChangeType((object)row[propertyInfo.Name.ToLower()].ToString(), propertyInfo.PropertyType));
                    }
                    listFromDataTable.Add(inventoryMoves);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return listFromDataTable;
        }

        public override string ToString()
        {
            string empty = string.Empty;
            foreach (PropertyInfo property in typeof(InventoryMoves).GetProperties())
                empty += string.Format("{0} -> {1} ", (object)property.Name, property.GetValue((object)this));
            return empty.Trim();
        }
    }
}
