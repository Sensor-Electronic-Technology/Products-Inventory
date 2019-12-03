using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventory.Common.EntityLayer.Model;
using Inventory.Common.EntityLayer.Model.Entities;
using Inventory.Common.DataLayer;
using Inventory.Common.DataLayer.Providers;
using Console_Table;
using System.Reflection;
using Inventory.Common.DataLayer.Services;
using System.Dynamic;
using System.Data;
using System.DirectoryServices.AccountManagement;
using System.Security.Cryptography;
using System.IO;
using Inventory.Common.ApplicationLayer;
using Inventory.Common.BuisnessLayer;
using System.Linq.Expressions;
using Inventory.Common.DataLayer.EntityDataManagers;
using Inventory.Common.DataLayer.EntityOperations;
using System.Data.Entity;

namespace Inventory.ConsoleTesting {
    public class Program2 {
        private static void Main(string[] args) {
            using(var context=new InventoryContext()) {

                Warehouse warehouse = new Warehouse();
                warehouse.Name = "Epi System Parts";
                warehouse.Description = "Storage room for replacement epi parts";
                context.Locations.Add(warehouse);


                Distributor distributor = new Distributor();
                distributor.Name = "Aixtron";
                distributor.Description = "Original Aixtron Parts";
                context.Distributors.Add(distributor);

                Contact contact = new Contact();
                contact.FirstName = "Marcy";
                contact.LastName = "Ripley";
                contact.Comments = "Aixtron Customer Service Representative";
                contact.Address = "1700 Wyatt Drive, Suite 15 Santa Clara, CA 95054";
                contact.Website = "www.aixtron.com";
                contact.Email = "M.Ripley@aixtron.com";
                contact.Phone = "669-228-3870";

                distributor.Contacts.Add(contact);
                context.Contacts.Add(contact);

                Part part = new Part("Diffusion Barrier","Diffusion Barriers");

                context.InventoryItems.Add(part);

                PartInstance barrierThin = new PartInstance(part, "2.22mm Barrier", "7-03-002-00-05R1", "", "");
                barrierThin.Quantity = 6;
                barrierThin.MinQuantity = 4;
                barrierThin.SafeQuantity = 8;

                part.Instances.Add(barrierThin);

                PartInstance barrierThick = new PartInstance(part, "3.33mm Barrier", "", "", "");
                barrierThick.Quantity = 2;
                barrierThick.MinQuantity = 0;
                barrierThick.SafeQuantity = 0;

                part.Instances.Add(barrierThick);



                Price price = new Price();
                price.Distributor = distributor;
                price.DistributorName = distributor.Name;
                price.Amount = 362;
                price.MinOrder = 1;
                price.LeadTime = 28;
                price.TimeStamp = DateTime.Now;

                distributor.Rates.Add(price);

                context.Rates.Add(price);
                context.SaveChanges();
                Console.WriteLine("Should be done");
                Console.ReadKey();
               
            }
        }
    }
}
