using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ModularMonolith
{
    public class Program
    {
        static void Main(string[] args)
        {
			var warehouses = new List<Warehouse> { new Warehouse() { ID = 1, Code = "WarehouseA" }, new Warehouse() { ID = 2, Code = "WarehouseB" } };
			var branches = new List<Branch> { new Branch() { ID = 1, Code = "BranchA", Warehouse = new Warehouse() { ID = 1, Code = "WarehouseA" } }, new Branch() { ID = 2, Code = "BranchB", Warehouse = new Warehouse() { ID = 1, Code = "WarehouseA" } } };
			var products = new List<Product> { new Product() { ID = 1, Barcode = "CoolChair" }, new Product() { ID = 2, Barcode = "CoolJacket" } };

			var warehouseStock = from w in warehouses
								 join p in products on 1 equals 1
								 select new ProductWarehouse() {
									 Warehouse = w,
									 Product = p,
									 HardQuantity = 5,
									 SoftQuantity = 5
								 };

			foreach (var warehouse in warehouses) {
				warehouse.WarehouseStock = warehouseStock.Where(x => x.Warehouse.ID == warehouse.ID).ToList();
			}

			var branchStocks = from b in branches
							  join p in products on 1 equals 1
							  select new ProductBranch() {
								  Branch = b,
								  Product = p,
								  HardQuantity = 0,
								  SoftQuantity = 0,
								  Required = 3
							  };

			// Simulate branch allocation
			while (true) {
				foreach(var branchStock in branchStocks) {
					if (branchStock.HardQuantity < branchStock.Required) {
						var actualWarehouseStock = branchStock.Branch.Warehouse.WarehouseStock.FirstOrDefault(x => x.Product.ID == branchStock.Product.ID);
						if (actualWarehouseStock.SoftQuantity == 0) {
							Console.WriteLine("Sorry all out!");
							continue;
						}

						actualWarehouseStock.SoftQuantity -= 1;
						branchStock.SoftQuantity += 1;
					}
				}

				Thread.Sleep(10000);
			}
        }
    }

	public class Warehouse {
		public int ID { get; set; }
		public string Code { get; set; }

		public IList<ProductWarehouse> WarehouseStock { get; set; }
	}

	public class Product {
		public int ID { get; set; }
		public string Barcode { get; set; }
	}

	public class Branch {
		public int ID { get; set; }		
		public string Code { get; set; }

		public Warehouse Warehouse { get; set; }
		public IList<ProductBranch> BranchStock { get; set; }
	}

	public class ProductBranch {
		public Product Product { get; set; }
		public Branch Branch { get; set; }
		public int SoftQuantity { get; set; }
		public int HardQuantity { get; set; }
		public int Required { get; set; }
	}

	public class ProductWarehouse {
		public Product Product { get; set; }
		public Warehouse Warehouse { get; set; }
		public int SoftQuantity { get; set; }
		public int HardQuantity { get; set; }
	}
}
