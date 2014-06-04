using System;
using MonoTouch.UIKit;
using System.Collections.Generic;
using MonoTouch.Foundation;
using POPpicLibrary;

namespace POPpiciOS
{
	/// <summary>
	/// Combined DataSource and Delegate for our UITableView
	/// </summary>
	public class GameListTableSource : UITableViewSource
	{
		//---- declare vars
		protected IList<GameViewModel> tableItems;
		protected string cellIdentifier = "GameTableCell";
		public event EventHandler<GameViewModel> GameSelected;

		public GameListTableSource (IList<GameViewModel> items)
		{
			tableItems = items;
		}

		#region data binding/display methods

		/// <summary>
		/// Called by the TableView to determine how many sections(groups) there are.
		/// </summary>
		public override int NumberOfSections (UITableView tableView)
		{
			return 1;
		}

		/// <summary>
		/// Called by the TableView to determine how many cells to create for that particular section.
		/// </summary>
		public override int RowsInSection (UITableView tableview, int section)
		{
			return tableItems.Count;
		}

		public POPpicLibrary.MyGamesViewModel.ListType ListType { get; set; }

		/// <summary>
		/// Called by the TableView to retrieve the header text for the particular section(group)
		/// </summary>
		public override string TitleForHeader (UITableView tableView, int section)
		{
			return MyGamesViewModel.GetGameTypeDescription(ListType);
		}

		public override float GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			var height = 70;
			return height;
		}

		/// <summary>
		/// Called by the TableView to retrieve the footer text for the particular section(group)
		/// </summary>
		public override string TitleForFooter (UITableView tableView, int section)
		{
			return "";
		}

		#endregion	

		#region user interaction methods

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			Console.WriteLine ("Row selected");
			if (this.GameSelected != null) {
				this.GameSelected (this, this.tableItems [indexPath.Row]);
			}

		}

		public override void RowDeselected (UITableView tableView, NSIndexPath indexPath)
		{
			Console.WriteLine ("Row " + indexPath.Row.ToString () + " deselected");	
		}

		#endregion	

		/// <summary>
		/// Called by the TableView to get the actual UITableViewCell to render for the particular section and row
		/// </summary>
		public override UITableViewCell GetCell (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			Console.WriteLine ("Calling Get Cell, isEditing:" + tableView.Editing);

			var game = this.tableItems [indexPath.Row];

			var cell = tableView.DequeueReusableCell (this.cellIdentifier) as TestTVC;
			if (cell == null) {
				cell =  TestTVC.Create();
			}
			cell.SetData(game);

			return cell;
		}
	}
}

