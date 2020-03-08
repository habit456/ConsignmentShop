using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ConsignmentShopLibrary;

namespace ConsignmentShopUI
{
    public partial class ConsignmentShop : Form
    {
        private Store store = new Store();
        private List<Item> shoppingCartData = new List<Item>();
        private decimal storeProfit = 0;

        BindingSource itemsBinding = new BindingSource();
        BindingSource cartBinding = new BindingSource();
        BindingSource vendorsBinding = new BindingSource();

        public ConsignmentShop()
        {
            InitializeComponent();
            SetupData();

            itemsBinding.DataSource = store.Items.Where(x => x.Sold == false).ToList();
            itemsListBox.DataSource = itemsBinding;

            itemsListBox.DisplayMember = "Display";
            itemsListBox.ValueMember = "Display";

            cartBinding.DataSource = shoppingCartData;
            shoppingCartListBox.DataSource = cartBinding;

            shoppingCartListBox.DisplayMember = "Display";
            shoppingCartListBox.ValueMember = "Display";

            vendorsBinding.DataSource = store.Vendors;
            vendorListBox.DataSource = vendorsBinding;

            vendorListBox.DisplayMember = "Display";
            vendorListBox.ValueMember = "Display";

            VendorListBoxLabel.Visible = false;
        }

        private void SetupData()
        {
            store.Vendors.Add(new Vendor { FirstName = "Bill", LastName = "Smith" });
            store.Vendors.Add(new Vendor { FirstName = "Sue", LastName = "Jones" });

            store.Items.Add(new Item
            {
                Title = "Moby Dick",
                Description = "A book about a whale.",
                Price = 4.50M,
                Owner = store.Vendors[0]
            });

            store.Items.Add(new Item
            {
                Title = "A Tale of Two Cities",
                Description = "A book about a revolution.",
                Price = 3.80M,
                Owner = store.Vendors[1]
            });

            store.Items.Add(new Item
            {
                Title = "Harry Potter (Book 1)",
                Description = "A book about a boy wizard.",
                Price = 5.20M,
                Owner = store.Vendors[1]
            });

            store.Items.Add(new Item
            {
                Title = "Jane Eyre",
                Description = "A book about a girl.",
                Price = 1.50M,
                Owner = store.Vendors[0]
            });

            store.Items.Add(new Item
            {
                Title = "15lb Hexagonal Dumbbell Set",
                Description = "Briefly used.",
                Price = 29.99M,
                Owner = store.Vendors[0]
            });

            store.Items.Add(new Item
            {
                Title = "Marilyn Manson Shirt",
                Description = "XL size, in new condition.",
                Price = 19.99M,
                Owner = store.Vendors[1]
            });

            store.Items.Add(new Item
            {
                Title = "Lightbulbs",
                Description = "Set of 20. Some don't work.",
                Price = 15.55M,
                Owner = store.Vendors[1]
            });

            store.Items.Add(new Item
            {
                Title = "Chair",
                Description = "Previously used by gamer. Covered in dorito dust. Cushion sunken in. Please, someone, buy this.",
                Price = 0.99M,
                Owner = store.Vendors[0]
            });

            store.StoreName = "Seconds Are Better";
        }

        private void addToCart_Click(object sender, EventArgs e)
        {
            if (itemsListBox.SelectedItem != null)
            {
                Item selectedItem = (Item)itemsListBox.SelectedItem;
                if (!selectedItem.InCart)
                {
                    selectedItem.InCart = true;
                    shoppingCartData.Add(selectedItem);
                    cartBinding.ResetBindings(false);
                }
                else
                {
                    MessageBox.Show(string.Format("{0} is in cart.", selectedItem.Title));
                }
            }
        }

        private void makePurchase_Click(object sender, EventArgs e)
        {
            decimal totalCost = CalculateTotalCost();

            if (totalCost > 0)
            {
                DialogResult dialogResult = MessageBox.Show(
                        string.Format("Total Cost: {0}. Make purchase?", totalCost.ToString("C")),
                        "Confirm",
                        MessageBoxButtons.YesNo);

                if (dialogResult == DialogResult.Yes)
                {
                    HandlePurchase();
                }
            }
        }

        private void HandlePurchase()
        {
            HandleItems();
            shoppingCartData.Clear();
            itemsBinding.DataSource = store.Items.Where(x => x.Sold == false).ToList();
            UpdateFrontEnd();
        }

        private void UpdateFrontEnd()
        {
            storeProfitValue.Text = storeProfit.ToString("C");
            cartBinding.ResetBindings(false);
            itemsBinding.ResetBindings(false);
            vendorsBinding.ResetBindings(false);
        }

        private void HandleItems()
        {
            foreach (Item item in shoppingCartData)
            {
                item.Sold = true;
                item.Owner.PaymentDue += item.Price * (decimal)item.Owner.Commission;
                storeProfit += (1 - (decimal)item.Owner.Commission) * item.Price;
            }
        }

        private decimal CalculateTotalCost()
        {
            decimal totalCost = 0;
            shoppingCartData.ForEach(x => totalCost += x.Price);
            return totalCost;
        }

        private void removeFromCartButton_Click(object sender, EventArgs e)
        {
            if (shoppingCartListBox.SelectedItem != null)
            {
                Item item = (Item)shoppingCartListBox.SelectedItem;
                shoppingCartData.Remove(item);
                item.InCart = false;
                cartBinding.ResetBindings(false);
            }
        }

        private void returnButton_Click(object sender, EventArgs e)
        {
            // storeInfoPanel.Visible = false;
        }

        private void storeInfoButton_Click(object sender, EventArgs e)
        {
            // storeInfoPanel.Visible = true;
        }
    }
}
