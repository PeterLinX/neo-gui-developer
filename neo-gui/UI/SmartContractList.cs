using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Neo.Core;
using System.IO;

namespace Neo.UI
{
    public partial class SmartContractList : Form
    {
        private UInt160 ignore;
        private UInt160 script_hash;
        private ListViewHitTestInfo listViewHitTestResult;      // populated by a click event on the listview
        bool listItemExists = false;
        public string selectedScriptHash;                       // store the script hash for a double click event


        public SmartContractList()
        {
            InitializeComponent();
            selectedScriptHash = "";
        }

        public void AddSmartContract(string scScripthash)
        {
            TimeSpan localTime = DateTime.Now.TimeOfDay;
            if (listViewSmartContracts.Items.ContainsKey(scScripthash)) return;
            else
            {
                listViewSmartContracts.Items.Insert(0, new ListViewItem(new[]
                    {
                    new ListViewItem.ListViewSubItem
                    {
                        Name = "Name",
                        Text = "(pending...)",
                        Font = new Font(SystemFonts.DefaultFont, FontStyle.Italic)
                    },
                    new ListViewItem.ListViewSubItem
                    {
                        Name = "Version",
                        Text = "0.0"
                    },
                    new ListViewItem.ListViewSubItem
                    {
                        Name = "Author",
                        Text = ".."
                    },
                    new ListViewItem.ListViewSubItem
                    {
                        Name = "Storage",
                        Text = ".."
                    },
                    new ListViewItem.ListViewSubItem
                    {
                        Name = "Status",
                        Text = "Unavailable ☹",
                        ForeColor = Color.Red
                    },
                    new ListViewItem.ListViewSubItem
                    {
                        Name = "Script Hash",
                        Text = scScripthash
                    },
                }, -1)
                {
                    UseItemStyleForSubItems = false,
                    Name = scScripthash
                });
            }
        }
        public void updateStatus()
        {
            foreach (ListViewItem Item in listViewSmartContracts.Items)
            {
                if (!UInt160.TryParse(Item.Name, out ignore)) continue;
                script_hash = UInt160.Parse(Item.Name);
                ContractState contract = Blockchain.Default.GetContract(script_hash);
                if (contract != null && Item.SubItems[0].Text != contract.Name)
                {
                    Item.SubItems[0].Text = contract.Name;
                    Item.SubItems[0].Font = SystemFonts.DefaultFont;
                    Item.SubItems[1].Text = contract.CodeVersion;
                    Item.SubItems[2].Text = contract.Author;
                    Item.SubItems[3].Text = contract.HasStorage.ToString();
                    Item.SubItems[4].Text = "Found! ツ";
                    Item.SubItems[4].Tag = contract.Description;
                    Item.SubItems[4].ForeColor = Color.Green;
                }
            }
            }

        private void CopySHtoolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewSmartContracts.SelectedItems.Count == 0) return;
            Clipboard.SetDataObject(listViewSmartContracts.SelectedItems[0].SubItems[3].Text);
        }

        private void SmartContractList_Load(object sender, EventArgs e)
        {
        }

        private void SmartContractList_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        private void listViewSmartContracts_MouseDown(object sender, MouseEventArgs e)
        {
            listViewHitTestResult = ((ListView)sender).HitTest(e.X, e.Y);
            listItemExists = listViewHitTestResult.Item != null;

            if (e.Button == MouseButtons.Left && e.Clicks == 2 && listItemExists)
            {
                selectedScriptHash = listViewHitTestResult.Item.SubItems[5].Text;
                if (Modal)
                {
                    // form was called as a modal dialog - set a result to return back to parent
                    DialogResult = DialogResult.OK;
                }
            }
        }

        private void showDescriptionMenuItem_Click(object sender, EventArgs e)
        {
            if (listItemExists)
            {
                MessageBox.Show(listViewHitTestResult.Item.SubItems[4].Tag.ToString(), listViewHitTestResult.Item.SubItems[0].Text);
            }
        }
    }
}
