using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;
using System.Management;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;
using System.ComponentModel;
using EO.WebBrowser;
namespace EncryptedPagesTool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public static EO.WebBrowser.DOM.Document document;
        public static string base64ImageRepresentation = "";
        private void Form1_Shown(object sender, EventArgs e)
        {
            this.pictureBox1.Dock = DockStyle.Fill;
            EO.WebEngine.BrowserOptions options = new EO.WebEngine.BrowserOptions();
            options.EnableWebSecurity = false;
            EO.WebBrowser.Runtime.DefaultEngineOptions.SetDefaultBrowserOptions(options);
            EO.WebEngine.Engine.Default.Options.AllowProprietaryMediaFormats();
            EO.WebEngine.Engine.Default.Options.SetDefaultBrowserOptions(new EO.WebEngine.BrowserOptions
            {
                EnableWebSecurity = false
            });
            this.webView1.Create(pictureBox1.Handle);
            this.webView1.Engine.Options.AllowProprietaryMediaFormats();
            this.webView1.SetOptions(new EO.WebEngine.BrowserOptions
            {
                EnableWebSecurity = false
            });
            this.webView1.Engine.Options.DisableGPU = false;
            this.webView1.Engine.Options.DisableSpellChecker = true;
            this.webView1.Engine.Options.CustomUserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko";
            Navigate("");
            string path = @"encryptedpagetool.html";
            string readText = DecryptFiles(path + ".encrypted", "tybtrybrtyertu50727885");
            webView1.LoadHtml(readText);
            webView1.RegisterJSExtensionFunction("GetBase64Image", new JSExtInvokeHandler(WebView_JSGetBase64Image));
        }
        public static string DecryptFiles(string inputFile, string password)
        {
            using (var input = File.OpenRead(inputFile))
            {
                byte[] salt = new byte[8];
                input.Read(salt, 0, salt.Length);
                using (var decryptedStream = new MemoryStream())
                using (var pbkdf = new Rfc2898DeriveBytes(password, salt))
                using (var aes = new RijndaelManaged())
                using (var decryptor = aes.CreateDecryptor(pbkdf.GetBytes(aes.KeySize / 8), pbkdf.GetBytes(aes.BlockSize / 8)))
                using (var cs = new CryptoStream(input, decryptor, CryptoStreamMode.Read))
                {
                    string contents;
                    int data;
                    while ((data = cs.ReadByte()) != -1)
                        decryptedStream.WriteByte((byte)data);
                    decryptedStream.Position = 0;
                    using (StreamReader sr = new StreamReader(decryptedStream))
                        contents = sr.ReadToEnd();
                    decryptedStream.Flush();
                    return contents;
                }
            }
        }
        private void webView1_LoadCompleted(object sender, LoadCompletedEventArgs e)
        {
            Task.Run(() => LoadPage());
        }
        private void LoadPage()
        {
            string stringinject;
            stringinject = @"
    <style>
body {
    font-family: sans-serif;
    background-color: #222222;
    color: #FFFFFF;
}

input {
    margin-top: 5px;
    margin-bottom: 5px;
    display: inline-block;
    vertical-align: middle;
    color: #000000;
}

div, textarea {
    word-wrap: break-word;
    hyphens: hidden;
    display: inline-block;
    width: 100%; 
    height: auto;
    max-width: 100%;
    overflow-x: hidden;
    overflow-y: auto;
}

#content, #contentcrypted {
    height: 450px;
    background-color: white;
    color: black;
}
    </style>
".Replace("\r\n", " ");
            stringinject = @"""" + stringinject + @"""";
            stringinject = @"$(" + stringinject + @" ).appendTo('head');";
            this.webView1.EvalScript(stringinject);
            stringinject = @"

    <form class='form-horizontal'>
        <input type='button' onClick='getBase64Image()' value='Get Base64 Image' class='button'>
    </form>

    <img id='image' src=''></img>

    <div id='imagebase64'></div>

    <form class='form-horizontal'>
        <input type='text' value='salt' class='text' id='salt'>
    </form>

    <form class='form-horizontal'>
        <input type='button' onClick='getContentCrypted()' value='Get Content Crypted' class='button'>
    </form>
    <textarea id='content'></textarea>

    <form class='form-horizontal'>
        <input type='button' onClick='getContentDecrypted()' value='Get Content Decrypted' class='button'>
    </form>
    <textarea id='contentcrypted'></textarea>

<script>

function getBase64Image() {
    GetBase64Image();
}

function getContentCrypted() {
    var salt = document.getElementById('salt').value;
    var content = document.getElementById('content').value;
    var contentcrypted = crypt(salt, btoa(content));
    document.getElementById('contentcrypted').value = contentcrypted;
}

function getContentDecrypted() {
    var salt = document.getElementById('salt').value;
    var contentcrypted = document.getElementById('contentcrypted').value;
    var content = decrypt(salt, contentcrypted);
    document.getElementById('content').value = atob(content);
}

const crypt = (salt, text) => {
  const textToChars = (text) => text.split('').map((c) => c.charCodeAt(0));
  const byteHex = (n) => ('0' + Number(n).toString(16)).substr(-2);
  const applySaltToChar = (code) => textToChars(salt).reduce((a, b) => a ^ b, code);
  return text
    .split('')
    .map(textToChars)
    .map(applySaltToChar)
    .map(byteHex)
    .join('');
};

const decrypt = (salt, encoded) => {
  const textToChars = (text) => text.split('').map((c) => c.charCodeAt(0));
  const applySaltToChar = (code) => textToChars(salt).reduce((a, b) => a ^ b, code);
  return encoded
    .match(/.{1,2}/g)
    .map((hex) => parseInt(hex, 16))
    .map(applySaltToChar)
    .map((charCode) => String.fromCharCode(charCode))
    .join('');
};

</script>
".Replace("\r\n", " ");
            stringinject = @"""" + stringinject + @"""";
            stringinject = @"$(document).ready(function(){$('body').append(" + stringinject + @");});";
            this.webView1.EvalScript(stringinject);
        }
        void WebView_JSGetBase64Image(object sender, JSExtInvokeArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Filter = "All Files(*.*)|*.*";
            if (op.ShowDialog() == DialogResult.OK)
            {
                byte[] imageArray = System.IO.File.ReadAllBytes(op.FileName);
                base64ImageRepresentation = Convert.ToBase64String(imageArray);
                Task.Run(() => SetImageBase64());
                try
                {
                    string stringinject = @"
                        document.getElementById('image').src = 'base64ImageRepresentation';
                    ";
                    this.webView1.QueueScriptCall(stringinject.Replace("base64ImageRepresentation", "data:image/jpg;base64, " + base64ImageRepresentation));
                }
                catch { }
            }
        }
        private void SetImageBase64()
        {
            try
            {
                document = webView1.GetDOMWindow().document;
                TraverseElementTree(document, (currentElement) =>
                {
                    string id = currentElement.GetID();
                    if (id.StartsWith("imagebase64") & id.EndsWith("imagebase64"))
                    {
                        currentElement.SetHtml("data:image/jpg;base64, " + base64ImageRepresentation);
                    }
                });
            }
            catch { }
        }
        private void TraverseElementTree(JSObject root, Action<JSObject> action)
        {
            action(root);
            foreach (var child in root.GetChildren())
                TraverseElementTree(child, action);
        }
        private void Navigate(string address)
        {
            webView1.Url = address;
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.webView1.Dispose();
        }
    }
    public static class JSObjectExtensions
    {
        public static void SetValue(this JSObject jsObj, string value)
        {
            jsObj["value"] = value;
        }
        public static void SetHtml(this JSObject jsObj, string value)
        {
            jsObj["innerHTML"] = value;
        }
        public static string GetValue(this JSObject jsObj)
        {
            return jsObj["value"] as string ?? string.Empty;
        }
        public static string GetHtml(this JSObject jsObj)
        {
            return jsObj["innerHTML"] as string ?? string.Empty;
        }
        public static string GetTagName(this JSObject jsObj)
        {
            return (jsObj["tagName"] as string ?? string.Empty).ToUpper();
        }
        public static string GetID(this JSObject jsObj)
        {
            return jsObj["id"] as string ?? string.Empty;
        }
        public static string GetAttribute(this JSObject jsObj, string attribute)
        {
            return jsObj.InvokeFunction("getAttribute", attribute) as string ?? string.Empty;
        }
        public static JSObject GetParent(this JSObject jsObj)
        {
            return jsObj["parentElement"] as JSObject;
        }
        public static IEnumerable<JSObject> GetChildren(this JSObject jsObj)
        {
            var childrenCollection = (JSObject)jsObj["children"];
            int childObjectCount = (int)childrenCollection["length"];
            for (int i = 0; i < childObjectCount; i++)
            {
                yield return (JSObject)childrenCollection[i];
            }
        }
    }
}