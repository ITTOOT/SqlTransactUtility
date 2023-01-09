using SqlTransactUtility;
using SqlTransactUtility.Entities;
using System;
using System.Linq;

namespace SqlTransactUtility
{
    public class LinqToEntityModel
    {
        public void efTest()
        {

            using (LinqToSQLDBEntities context = new LinqToSQLDBEntities())
            {
                //Get the List of Departments from Database
                var testItemList = from t in context.TestItems
                                   select t;

                foreach (var item in testItemList)
                {
                    Console.WriteLine("Department Id = {0} , Department Name = {1}",
                       item.SerialNo, item.Name);
                }

                //Add new Department
                TestItem testItem = new TestItem();
                testItem.Name = "Support";

                context.TestItems.Add(testItem);
                context.SaveChanges();

                Console.WriteLine("Department Name = Support is inserted in Database");

                ////Update existing Department
                //TestItem updateTestItem = context.TestItems.FirstOrDefault(t ⇒ t.RowNum == 1);
                //updateTestItem.Description = "Description updated";
                //context.SaveChanges();

                //Console.WriteLine("Department Name = Account is updated in Database");

                ////Delete existing Department
                //TestItem deleteTestItem = context.TestItems.FirstOrDefault(t ⇒ t.RowNum == 3);
                //context.TestItems.Remove(deleteTestItem);
                //context.SaveChanges();

                Console.WriteLine("Department Name = Pre-Sales is deleted in Database");

                //Get the Updated List of Departments from Database
                testItemList = from d in context.TestItems
                               select d;

                foreach (var item in testItemList)
                {
                    Console.WriteLine("Department Id = {0} , Department Name = {1}",
                       item.SerialNo, item.Name);
                }
            }

            Console.WriteLine("\nPress any key to continue.");
            Console.ReadKey();
        }
    }
}
