﻿using OfficeOpenXml.Table;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using System.Data;
using WebRazor.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace WebRazor.Helpers
{
	public class ExportProduct
	{
		// Initialize the database context for Entity Framework
		private static readonly PRN221DBContext _context = new PRN221DBContext();

		// Method to export data to an Excel file
		public static Stream? ExportExcelFile(string tableNeedExport, DateTime? dateFrom, DateTime? dateTo, string txtSearch)
		{
			DataTable dataTable;

			// Check if the export is for products or customers
			if (tableNeedExport.Equals("product"))
			{
				dataTable = getProductToExport(txtSearch);
			}
			else
			{
				dataTable = getCustomerToExport(dateFrom, dateTo);
			}

			// If no data to export, return an empty stream
			if (dataTable == null)
			{
				return Stream.Null;
			}

			// Create an Excel package in memory
			using (var excelPackage = new ExcelPackage(new MemoryStream()))
			{
				// Set properties for the Excel workbook
				excelPackage.Workbook.Properties.Author = "Hanker";
				excelPackage.Workbook.Properties.Title = "EPP test background";
				excelPackage.Workbook.Properties.Comments = "This is my generated Comments";

				// Add a worksheet to the Excel workbook
				var workSheet = tableNeedExport.Equals("product")
					? excelPackage.Workbook.Worksheets.Add("Products")
					: excelPackage.Workbook.Worksheets.Add("Orders");

				// Load data into the Excel worksheet
				workSheet.Cells.LoadFromDataTable(dataTable, true);

				// Save the Excel package and return the stream
				excelPackage.Save();
				return excelPackage.Stream;
			}
		}

		// Method to get customer data for export
		private static DataTable? getCustomerToExport(DateTime? dateFrom, DateTime? dateTo)
		{
			// Query orders with associated customer and employee data
			var orders = from o in _context.Orders
						 .Include(o => o.Customer)
						 .Include(o => o.Employee)
						 select o;

			List<Order> orderList;

			// Filter orders based on date range
			if (DateTime.Compare(dateFrom.Value, dateTo.Value) == 0)
			{
				orderList = orders.Where(c => c.OrderDate.Value == dateFrom.Value).ToList();
			}
			else
			{
				orderList = orders.Where(o => DateTime.Compare(o.OrderDate.Value, dateFrom.Value) >= 0
					&& DateTime.Compare(o.OrderDate.Value, dateTo.Value) <= 0).ToList();
			}

			// If orders exist, create and populate a DataTable
			if (orderList != null)
			{
				DataTable dataTable = new DataTable();
				// Define columns for the DataTable
				dataTable.Columns.Add("#", typeof(string));
				dataTable.Columns.Add("Customer", typeof(string));
				dataTable.Columns.Add("Employee", typeof(string));
				dataTable.Columns.Add("OrderDate", typeof(string));
				dataTable.Columns.Add("RequiredDate", typeof(string));
				dataTable.Columns.Add("ShippedDate", typeof(string));
				dataTable.Columns.Add("Freight", typeof(string));
				dataTable.Columns.Add("ShipName", typeof(string));
				dataTable.Columns.Add("ShipAddress", typeof(string));
				dataTable.Columns.Add("ShipCity", typeof(string));
				dataTable.Columns.Add("ShipRegion", typeof(string));
				dataTable.Columns.Add("ShipPostalCode", typeof(string));

				int count = 1;

				// Populate the DataTable with order data
				foreach (var o in orderList)
				{
					DataRow row = dataTable.NewRow();
					row[0] = count;
					row[1] = o.Customer == null ? "" : o.Customer.ContactName;
					row[2] = o.Employee == null ? "" : (o.Employee.FirstName + " " + o.Employee.LastName);
					row[3] = o.OrderDate == null ? "" : o.OrderDate.Value.ToString("dd/MM/yyyy");
					row[4] = o.RequiredDate == null ? "" : o.RequiredDate.Value.ToString("dd/MM/yyyy");
					row[5] = o.ShippedDate == null ? "" : o.ShippedDate.Value.ToString("dd/MM/yyyy");
					row[6] = o.Freight;
					row[7] = o.ShipName;
					row[8] = o.ShipAddress;
					row[9] = o.ShipCity;
					row[10] = o.ShipRegion;
					row[11] = o.ShipPostalCode;
					dataTable.Rows.Add(row);
					count++;
				}

				return dataTable;
			}

			return null;
		}

		// Method to get product data for export
		private static DataTable? getProductToExport(string txtSearch)
		{
			// Query products with associated category data
			var pro = (from p in _context.Products.Include(p => p.Category) select p);
			List<Product> productList;

			int idCategory = Int32.Parse(txtSearch);

			// Filter products based on category ID
			if (idCategory > 0)
			{
				productList = pro.Where(p => p.CategoryId == idCategory).ToList();
			}
			else
			{
				productList = pro.ToList();
			}

			// If products exist, create and populate a DataTable
			if (productList != null)
			{
				DataTable dataTable = new DataTable();
				// Define columns for the DataTable
				dataTable.Columns.Add("#", typeof(string));
				dataTable.Columns.Add("ProductName", typeof(string));
				dataTable.Columns.Add("QuantityPerUnit", typeof(string));
				dataTable.Columns.Add("Category", typeof(string));
				dataTable.Columns.Add("UnitPrice", typeof(string));
				dataTable.Columns.Add("UnitsInStock", typeof(string));
				dataTable.Columns.Add("UnitsOnOrder", typeof(string));
				dataTable.Columns.Add("ReorderLevel", typeof(string));

				int count = 1;

				// Populate the DataTable with product data
				foreach (var product in productList)
				{
					DataRow row = dataTable.NewRow();
					row[0] = count;
					row[1] = product.ProductName;
					row[2] = product.QuantityPerUnit;
					row[3] = product.Category == null ? "" : product.Category.CategoryName;
					row[4] = product.UnitPrice;
					row[5] = product.UnitsInStock;
					row[6] = product.UnitsOnOrder;
					row[7] = product.ReorderLevel;
					dataTable.Rows.Add(row);
					count++;
				}

				return dataTable;
			}

			return null;
		}
	}
}
