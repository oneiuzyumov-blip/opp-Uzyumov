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
        private const string FilePath = "todos.json";
        private TodoList _list = new TodoList();
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                _list.Load(FilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось загрузить задачи: " + ex.Message);
            }
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
                _list.Save(FilePath);
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
                _list.Save(FilePath);
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
                _list.Save(FilePath);
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
                _list.Save(FilePath);
                RefreshList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


    }
}