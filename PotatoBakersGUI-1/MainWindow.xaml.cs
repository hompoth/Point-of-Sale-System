using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;



namespace PotatoBakersGUI_1
{
    public partial class MainWindow : Window
    {
        string connectionString = @"Data Source=CAITLIN-PC;Initial Catalog=CFdatabase;Integrated Security=True;Pooling=False";
        List<double> prices = new List<double>(); // store prices to send to order preview
        public class order // class info for order preview
        {
            public string name = "";
            public double price = 0;
            public int quantity = 0;
            public string toString(){ // display for order preview/output
                return name + " x" + quantity + "\t\t\t\t\t" + quantity * price;
            }
        }
        List<order> orders = new List<order>(); // store info for order preview
        double subtotal = 0;
        double tax;
        double amountDue;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void addButton_Click(object sender, RoutedEventArgs e) // on add button click
        {
            if (quantityBox.Text != "" && AllItems.SelectedIndex >= 0) // check if quantity of items > 0 & current selection exists
            {
                int value = Convert.ToInt32(quantityBox.Text), newItem = 1;
                double price = prices[AllItems.SelectedIndex];
                for (int i = 0; i < orders.Count && newItem == 1; ++i) // search through all current orders
                {
                    if (orders[i].name.Equals(AllItems.SelectedItem.ToString())) // if order being added is already listed, append the quantity
                    {
                        newItem = 0;
                        orders[i].quantity += value;
                        orderPreview.Items[i] = orders[i].toString();
                    }
                }
                if (newItem == 1) // if this is a new order, add it to the list
                {
                    orders.Add(new order() { name = AllItems.SelectedItem.ToString(), price = price, quantity = value });
                    orderPreview.Items.Add(orders[orders.Count - 1].toString());
                }
                subtotal += value * price;
                tax = subtotal * 0.13;
                amountDue = subtotal + tax;
                updateTotals();
                quantityBox.Text = "1";
            }
        }
        private void removeButton_Click(object sender, RoutedEventArgs e) // on remove button click
        {
            int index = orderPreview.SelectedIndex;
            if (index >= 0 && quantityBox.Text != "") // check if quantity of items > 0 & current selection exists
            {
                int amt = Convert.ToInt32(quantityBox.Text);
                if (orders[orderPreview.SelectedIndex].quantity <= amt) // if current item's quantity is less than being removed, then remove all 
                {
                    subtotal -= orders[index].price * orders[index].quantity;
                    orders.RemoveAt(index);
                    orderPreview.Items.RemoveAt(index);
                }
                else // if current item's quantity is more than being removed, remove only that amount
                {
                    subtotal -= orders[index].price * amt;
                    orders[index].quantity -= amt;
                    orderPreview.Items[index] = orders[index].toString();
                }
                tax = subtotal * 0.13;
                amountDue = subtotal + tax;
                updateTotals();
            }
        }
        private void SpecialtyRoll_Click(object sender, RoutedEventArgs e) // on button click, call function listItems() below
        {

            listItems("SpecialtyRoll");
        }
        private void Maki_Click(object sender, RoutedEventArgs e)
        {
            listItems("Maki");
        }

        private void Gunkan_Click(object sender, RoutedEventArgs e)
        {
            listItems("Gunkan");
        }
        private void HandRoll_Click(object sender, RoutedEventArgs e)
        {
            listItems("HandRoll");
        }

        private void Nigiri_Click(object sender, RoutedEventArgs e)
        {
            listItems("Nigiri");
        }

        private void FlamedN_Click(object sender, RoutedEventArgs e)
        {
            listItems("FlamedN");
        }

        private void Sashimi_Click(object sender, RoutedEventArgs e)
        {
            listItems("Sashimi");
        }

        private void Appetizer_Click(object sender, RoutedEventArgs e)
        {
            listItems("Appetizers");
        }

        private void Combos_Click(object sender, RoutedEventArgs e)
        {

        }
        private void listItems(string tableName) // called on all categories and display the contents
        {
            var conn = new SqlConnection(connectionString);
            try
            {

                int count = AllItems.Items.Count;

                if (count > 0) AllItems.Items.Clear(); // clear the list
                /*//if the list isn't empty, we first clear it
                if (AllItems.Items.Count > 0)
                {
                    for (int i = count - 1; i >= 0; i--)
                    {
                        AllItems.Items.RemoveAt(i);
                    }
                }*/
                //now repopulate list box using database table corresponding to the button
                conn.Open();
                string query = "select * from " + tableName;
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.ExecuteNonQuery();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read()) // read row
                {
                    string item = dr.GetInt32(0) + " " + dr.GetString(1) + "\t" + dr.GetDouble(2); // retrieve column values and turn into string
                    prices.Add(dr.GetDouble(2)); // save price
                    AllItems.Items.Add(item); // save to category list
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.ToString());
            }
        }
        private void updateTotals() // update current totals
        {
            int count = totalBox.Items.Count;
            if (count > 0) totalBox.Items.Clear(); // clear old list
            /*{
                for (int i = count - 1; i >= 0; i--)
                {
                    totalBox.Items.RemoveAt(i);
                }
            }*/
            totalBox.Items.Add("subtotal:\t\t\t\t" + subtotal.ToString("F")); // enter new list of totals
            totalBox.Items.Add("tax:\t\t\t\t" + tax.ToString("F"));
            totalBox.Items.Add("Amount Due:\t\t\t" + amountDue.ToString("F"));
        }
        private void printButton_Click(object sender, RoutedEventArgs e) // print to console
        {
            foreach (string i in orderPreview.Items)
            {
                Console.WriteLine(i); // print order preview items

            }
            foreach (string i in totalBox.Items)
            {
                Console.WriteLine(i); // print order total items
            }
        }
    }
}

