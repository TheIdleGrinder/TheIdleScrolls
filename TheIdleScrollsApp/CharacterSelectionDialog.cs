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
            CharacterName = name;

            InitializeComponent();

            UpdateCharacterList();
            inputName.Text = CharacterName;
        }

        private void UpdateCharacterList()
        {
            m_characters = new() { "A", "B", "C" };
            listBoxChars.Items.Clear();
            foreach (string name in m_characters)
            {
                listBoxChars.Items.Add(name);
            }
            btnSelect.Enabled = m_characters.Any();
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
                
            }
            CharacterName = name;
            Close();
        }
    }
}
