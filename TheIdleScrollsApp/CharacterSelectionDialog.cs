using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheIdleScrolls_Core.DataAccess;

namespace TheIdleScrollsApp
{
    public partial class CharacterSelectionDialog : Form
    {
        //private DataAccessHandler m_accessHandler;

        private List<string> m_characters = new();

        public string CharacterName { get; set; } = "Leeroy";

        public CharacterSelectionDialog(string name)
        {
            //m_accessHandler = accessHandler;
            InitializeComponent();

            CharacterName = inputName.Text = name;
            UpdateCharacterList();
        }

        private void UpdateCharacterList()
        {
            m_characters = new() { "Existing Characters", "=> Coming soon", " ", "Load a character by", "entering their name below" };
            listBoxChars.Items.Clear();
            foreach (string name in m_characters)
            {
                listBoxChars.Items.Add(name);
            }
            btnSelect.Enabled = false;// m_characters.Any();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            CharacterName = listBoxChars.Text;
            Close();
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            string name = inputName.Text;
            if (m_characters.Contains(name))
            {
                // Ask whether to overwrite the character
            }
            CharacterName = name;
            Close();
        }
    }
}
