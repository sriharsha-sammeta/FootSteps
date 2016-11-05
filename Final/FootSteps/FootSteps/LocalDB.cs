using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Windows.Storage;
using SQLite;
using System.IO;
using System.Windows;

namespace FootSteps
{
    class LocalDB
    {
        public static string DB_PATH = Path.Combine(Path.Combine(ApplicationData.Current.LocalFolder.Path, "contrack.sqlite"));
        public static SQLiteConnection dbConn;

        public static bool exists()
        {
            try
            {
                dbConn.Query<LocalPerson>("SELECT * FROM localperson");
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public static void createLocalPerson()
        {
            dbConn = new SQLiteConnection(DB_PATH);
            dbConn.CreateTable<LocalPerson>();
        }

        public static void insertLocalPerson(LocalPerson lp)
        {
            
            // Insert the new task in the Task table.  
            dbConn.Insert(lp);
            
        }
        public static void retrieveLocalPerson()
        {
            // Retriving Data  
            var tp = dbConn.Query<LocalPerson>("select * from localperson where Id='12345678'").FirstOrDefault();
            if (tp == null)
                MessageBox.Show("Title Not Present in DataBase");
            else
                MessageBox.Show("Id 12345678 exists");
        }
        public static void deleteLocalPerson()
        {
            // Deleting Entire Row from DB by matching Title Filed  
            var tp = dbConn.Query<LocalPerson>("delete from localperson");
            // Check result is empty or not  
            
        }  
    }
}
