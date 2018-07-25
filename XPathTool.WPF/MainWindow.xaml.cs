using HtmlAgilityPack;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace XPathTool.WPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private HtmlDocument _document;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void btn_fromUrl_Load_Click(object sender, RoutedEventArgs e)
        {
            if (!HasText(tb_fromUrl))
            {
                MessageBox.Show("Url is not set.");
                return;
            }
            btn_fromUrl_Load.IsEnabled = false;
            try
            {
                using (var client = new HttpClient())
                {
                    var html = await client.GetStringAsync(tb_fromUrl.Text);

                    _document = new HtmlDocument();
                    _document.LoadHtml(html);
                    tb_status_htmlDoc.Text = "Ready";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                btn_fromUrl_Load.IsEnabled = true;
            }
        }

        private void btn_fromFile_Load_Click(object sender, RoutedEventArgs e)
        {
            btn_fromFile_Load.IsEnabled = false;
            try
            {
                OpenFileDialog dlg = new OpenFileDialog
                {
                    Filter = "Html file (.html)|*.html|All files|*.*",
                    FilterIndex = 0
                };

                if (dlg.ShowDialog() == true)
                {
                    _document = new HtmlDocument();
                    _document.Load(dlg.FileName);
                    tb_status_htmlDoc.Text = "Ready";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                btn_fromFile_Load.IsEnabled = true;
            }
        }

        private void btn_fromHtml_Load_Click(object sender, RoutedEventArgs e)
        {
            if (!HasText(tb_fromHtml))
            {
                MessageBox.Show("Html is not set.");
                return;
            }
            btn_fromHtml_Load.IsEnabled = false;
            try
            {
                _document = new HtmlDocument();
                _document.LoadHtml(tb_fromHtml.Text);
                tb_status_htmlDoc.Text = "Ready";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                btn_fromHtml_Load.IsEnabled = true;
            }
        }

        private static bool HasText(TextBox textBox) => textBox != null && !string.IsNullOrWhiteSpace(textBox.Text);

        private void btn_xpath_parse_Click(object sender, RoutedEventArgs e)
        {
            if (!HasText(tb_xpath))
            {
                MessageBox.Show("XPath is not set.");
                return;
            }
            if (_document != null)
            {
                var nodes = _document.DocumentNode.SelectNodes(tb_xpath.Text);
                if (nodes == null || nodes.Count == 0)
                {
                    MessageBox.Show($"Not found any html nodes for given XPath: {tb_xpath.Text}.");
                    return;
                }

                var nodeHtmls = nodes.Select(n => n.OuterHtml);
                StringBuilder sb = new StringBuilder();
                foreach (var html in nodeHtmls)
                {
                    sb.AppendLine(html);
                }
                tb_output.Text = sb.ToString();
            }
        }
    }
}
