using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary
{
    public class Group
    {
        public string SGrupo { get; set; }

        public string SDescripcion { get; set; }

        public string SGrupo_Padre { get; set; }

        public Group()
        {
        }

        public Group(string pgrupo, string pdescipcion, string pgrupo_padre)
        {
            this.SGrupo = pgrupo;
            this.SDescripcion = pdescipcion;
            this.SGrupo_Padre = pgrupo_padre;
        }

        public static List<Group> getListFromDataTable(DataTable group)
        {
            List<Group> groupList = new List<Group>();
            PropertyInfo[] properties = typeof(Group).GetProperties();
            try
            {
                foreach (DataRow row in (InternalDataCollectionBase)group.Rows)
                {
                    Group group1 = new Group();
                    foreach (PropertyInfo propertyInfo in properties)
                    {
                        if (!string.IsNullOrEmpty(row[propertyInfo.Name.ToLower()].ToString()))
                            propertyInfo.SetValue((object)group1, (object)row[propertyInfo.Name.ToLower()].ToString());
                    }
                    groupList.Add(group1);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return groupList;
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
