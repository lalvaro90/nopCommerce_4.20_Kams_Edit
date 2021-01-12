using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary
{
    public class PCGRAF_Product
    {
        public string SCodigo_Producto { get; set; }

        public string SDescripcion_Inventario { get; set; }

        public string SGrupo { get; set; }

        public string P1 { get; set; }

        public string P2 { get; set; }

        public string P3 { get; set; }

        public string P4 { get; set; }

        public PCGRAF_Product()
        {
        }

        public PCGRAF_Product(
          string pcodigo_producto,
          string pdescripcion_inventario,
          string pgrupo,
          string pprecio_publico)
        {
        }

        public static List<PCGRAF_Product> getListFromDataTable(DataTable group)
        {
            List<PCGRAF_Product> productList = new List<PCGRAF_Product>();
            PropertyInfo[] properties = typeof(PCGRAF_Product).GetProperties();
            try
            {
                foreach (DataRow row in (InternalDataCollectionBase)group.Rows)
                {
                    PCGRAF_Product product = new PCGRAF_Product();
                    foreach (PropertyInfo propertyInfo in properties)
                    {
                        if (!string.IsNullOrEmpty(row[propertyInfo.Name.ToLower()].ToString()))
                            propertyInfo.SetValue((object)product, (object)row[propertyInfo.Name.ToLower()].ToString());
                    }
                    productList.Add(product);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return productList;
        }

        public override string ToString()
        {
            string empty = string.Empty;
            foreach (PropertyInfo property in typeof(Product).GetProperties())
                empty += string.Format("{0} -> {1} ", (object)property.Name, property.GetValue((object)this));
            return empty.Trim();
        }
    }
}
