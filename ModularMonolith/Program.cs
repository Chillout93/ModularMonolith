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
			var branches = new List<Branch> { new Branch() { ID = 1, Code = "BranchA" }, new Branch() { ID = 2, Code = "BranchB" } };
			var products = new List<Product> { new Product() { ID = 1, Barcode = "CoolChair" }, new Product() { ID = 2, Barcode = "CoolJacket" } };
			var productGroups = new List<ProductGroup>();
			var branchGroups = new List<BranchGroup>();

			foreach(var product in products) {
				productGroups.Add(new ProductGroup() { Product = product, GroupCode = "MAIN" });

				product.ProductGroups = productGroups;
			}

			foreach (var branch in branches) {
				branchGroups.Add(new BranchGroup() { Branch = branch, GroupCode = "MAIN" });
				branchGroups.Add(new BranchGroup() { Branch = branch, GroupCode = "HIGHWINTER" });

				branch.BranchGroups = branchGroups;
			}
			
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

			foreach (var branch in branches) {
				branch.BranchStock = branchStocks.Where(x => x.Branch.ID == branch.ID).ToList();
				branch.Warehouse = warehouses.FirstOrDefault(x => x.ID == 1);
			}

			// Simulate branch allocation
			while (true) {
				foreach(var branchStock in branchStocks.Where(x => x.Branch.BranchGroups.Any(y => y.GroupCode == "HIGHWINTER"))) {
					if (branchStock.HardQuantity < branchStock.Required) {
						var actualWarehouseStock = branchStock.Branch.Warehouse.WarehouseStock.FirstOrDefault(x => x.Product.ID == branchStock.Product.ID);
						if (actualWarehouseStock.SoftQuantity == 0) {
							Console.WriteLine("Sorry all out!");
							continue;
						}

						actualWarehouseStock.SoftQuantity -= 1;
						branchStock.SoftQuantity += 1;

						Console.WriteLine($"{actualWarehouseStock.Warehouse.Code} now {actualWarehouseStock.SoftQuantity}, {branchStock.Branch.Code} now {branchStock.SoftQuantity}");
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

		public IList<ProductGroup> ProductGroups { get; set; }
	}

	public class ProductGroup {
		public string GroupCode { get; set; }
		public Product Product { get; set; }
	}

	public class BranchGroup {
		public string GroupCode { get; set; }
		public Branch Branch { get; set; }
	}

	public class Branch {
		public int ID { get; set; }		
		public string Code { get; set; }

		public Warehouse Warehouse { get; set; }
		public IList<ProductBranch> BranchStock { get; set; }
		public IList<BranchGroup> BranchGroups { get; set; }
 	}

	public class ProductBranch {
		public Product Product { get; set; }
		public Branch Branch { get; set; }
		public int SoftQuantity { get; set; }
		public int HardQuantity { get; set; }
		public int Required { get; set; }

		public IList<ProductGroup> ProductGroups { get; set; }
	}

	public class ProductWarehouse {
		public Product Product { get; set; }
		public Warehouse Warehouse { get; set; }
		public int SoftQuantity { get; set; }
		public int HardQuantity { get; set; }
	}
}
