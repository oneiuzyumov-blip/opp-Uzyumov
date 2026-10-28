using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Wpf
{

    public partial class MainWindow : Window
    {
        private TodoList _list = new TodoList();
        public MainWindow()
        {
            InitializeComponent();
            RefreshList();

        }

        private void RefreshList()
        {
            listBox.Items.Clear();
            foreach (TodoItem item in _list.Items)
            {
                ListBoxItem row = new ListBoxItem();
                row.Content = item.ToString();
                row.Tag = item;
                listBox.Items.Add(row);
            }
        }

        private TodoItem GetSelectedItem()
        {
            ListBoxItem? row = listBox.SelectedItem as ListBoxItem;
            if (row == null)
                throw new InvalidOperationException("Выберите задачу в списке");
            return (TodoItem)row.Tag;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int priority;
                if (!int.TryParse(priorityBox.Text, out priority))
                    throw new ArgumentException("Приоритет должен быть числом");

                _list.Add(titleBox.Text, priority);
                titleBox.Clear();
                RefreshList();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }
        private void CompleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TodoItem item = GetSelectedItem();
                item.Complete();
                RefreshList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void RenameButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TodoItem item = GetSelectedItem();
                item.Rename(titleBox.Text);
                titleBox.Clear();
                RefreshList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TodoItem item = GetSelectedItem();
                _list.Remove(item);
                RefreshList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}