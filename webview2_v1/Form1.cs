using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace webview2_v1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            string str = Directory.GetCurrentDirectory() + "\\Page\\img\\logo_dfr.png";
            webView21.BackgroundImage = Image.FromFile(str);
           
            string curDir = "file:///" + Directory.GetCurrentDirectory() + "\\Page\\index.html";
            webView21.Source = new Uri(curDir);
            InitializeAsync();  // inicializa la escucha de la funcion desde JS
        }
        async void InitializeAsync()
        {
            await webView21.EnsureCoreWebView2Async(null);
            webView21.CoreWebView2.WebMessageReceived += CallCSharp;
        }
        void CallCSharp(object sender, CoreWebView2WebMessageReceivedEventArgs args)
        {
            String message = args.TryGetWebMessageAsString();
            //MessageBox.Show("Mensaje generado desde JS "+ message, "Atencion");
            //webView21.CoreWebView2.PostWebMessageAsString(uri);
            if (message == "") {
                if (serialPort1.IsOpen == true && serialPort1.BytesToRead > 0)
                {
                    string received = serialPort1.ReadExisting();
                    string obj = $"messageFromCsharp({received})";
                    webView21.ExecuteScriptAsync(obj);          // convierte el json en object de JS
                    serialPort1.DiscardInBuffer();
                }
            }
            if (serialPort1.IsOpen == true && message != "")
            {
                serialPort1.Write(message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (button2.Text == "CONECTAR")
                {
                    serialPort1.Dispose();
                    serialPort1.BaudRate = 38400;
                    serialPort1.PortName = "COM" + textBox1.Text;
                    serialPort1.Open();
                    //MessageBox.Show("COM" + textBox1.Text + " Abierto");
                    panel1.Visible = false;
                }
                else
                {
                    panel1.Visible = true;
                    serialPort1.Close();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo abrir el puerto, verifique que el dispositivo esta conectado", " ERROR !!");
            }
        }
    }
}
