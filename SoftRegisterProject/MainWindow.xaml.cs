using Newtonsoft.Json;
using SoftRegisterProject.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SoftRegisterProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        HttpClient client = new HttpClient();
        public MainWindow()
        {
            client.BaseAddress = new Uri("https://testeapisr-eee3gxate2adfph6.brazilsouth-01.azurewebsites.net/api/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            InitializeComponent();
            this.GetContacts();
        }

        private void btnLoadGrid_Click(object sender, RoutedEventArgs e)
        {
            this.GetContacts();

        }

        private async void GetContacts()
        {
            var message = await client.GetStringAsync("Contacts");
            var contacts = JsonConvert.DeserializeObject<List<Contacts>>(message);
            dataGridContacts.ItemsSource = contacts;
        }

        private async void SaveContact(Contacts contact)
        {
            await client.PostAsJsonAsync("Contacts", contact);
        }
        private async void UpdateContact(Contacts contact)
        {
            await client.PutAsJsonAsync("Contacts/" + contact.ContactId, contact);
        }
        private async void DeleteContact(int studentId)
        {
            await client.DeleteAsync("Contacts/" + studentId);
        }
        private void btnSaveContact_Click(object sender, RoutedEventArgs e)
        {

            if (HasEmptyFields())
            {
                MessageBox.Show("Favor preencher todos os campos!", "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var contact = new Contacts
            {
                ContactId = string.IsNullOrEmpty(textBoxId.Text) ? 0 : Convert.ToInt32(textBoxId.Text),
                FirstName = textBoxName.Text,
                LastName = textBoxLastName.Text,
                PhoneNumber = textBoxPhone.Text,
            };


            if (contact.ContactId == 0) { 
                this.SaveContact(contact);
                MessageBox.Show("Cadastro realizado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            else
            {
                this.UpdateContact(contact);
                MessageBox.Show("Cadastro atualizado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            textBoxId.Text = string.Empty;

            textBoxId.Visibility = Visibility.Hidden;
            labelId.Visibility = Visibility.Hidden;
            btnSaveContact.Content = "Cadastrar Contato";


            textBoxName.Text = string.Empty;
            textBoxLastName.Text = string.Empty;
            textBoxPhone.Text = string.Empty;

            this.GetContacts();
        }

        void btnEditContact(object sender, RoutedEventArgs e)
        {
            textBoxId.Visibility = Visibility.Visible;
            labelId.Visibility = Visibility.Visible;

            btnSaveContact.Content = "Atualizar Contato";

            Contacts contact = ((FrameworkElement)sender).DataContext as Contacts;

            textBoxId.Text = contact.ContactId.ToString();
            textBoxName.Text = contact.FirstName;
            textBoxLastName.Text = contact.LastName;
            textBoxPhone.Text = contact.PhoneNumber;

            this.GetContacts();
        }
        void btnDeleteContact(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Deseja excluir esse registro? Essa ação é irreversível!", "Confirmação", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Contacts contact = ((FrameworkElement)sender).DataContext as Contacts;
                this.DeleteContact(contact.ContactId);
                this.GetContacts();
            }
        }

        private void btnReloadFormClick(object sender, RoutedEventArgs e)
        {
            textBoxId.Text = string.Empty;
            textBoxName.Text = string.Empty;
            textBoxLastName.Text = string.Empty;
            textBoxPhone.Text = string.Empty;

            textBoxId.Visibility = Visibility.Hidden;
            labelId.Visibility = Visibility.Hidden;

            btnSaveContact.Content = "Cadastrar Contato";
            
        }

        private bool HasEmptyFields()
        {
            return string.IsNullOrWhiteSpace(textBoxName.Text) ||
                   string.IsNullOrWhiteSpace(textBoxLastName.Text) ||
                   string.IsNullOrWhiteSpace(textBoxPhone.Text);
        }

        private void TextBoxPhone_TextChanged(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            e.Handled = !Regex.IsMatch(e.Text, "[0-9]");

            if (!e.Handled)
            {
                string text = textBox.Text + e.Text;

                text = Regex.Replace(text, "[^0-9]", "");

                if (text.Length > 11)
                {
                    e.Handled = true;
                    return;
                }

                if (text.Length <= 10)
                {
                    if (text.Length > 6)
                        text = $"({text.Substring(0, 2)}) {text.Substring(2, 4)}-{text.Substring(6)}";
                    else if (text.Length > 2)
                        text = $"({text.Substring(0, 2)}) {text.Substring(2)}";
                    else
                        text = $"({text}";
                }
                else
                {
                    if (text.Length > 7)
                        text = $"({text.Substring(0, 2)}) {text.Substring(2, 5)}-{text.Substring(7)}";
                    else if (text.Length > 2)
                        text = $"({text.Substring(0, 2)}) {text.Substring(2)}";
                    else
                        text = $"({text}";
                }

                textBox.Text = text;

                textBox.CaretIndex = text.Length;

                e.Handled = true; 
            }
        }
    }
}