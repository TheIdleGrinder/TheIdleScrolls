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
        private DataAccessHandler m_accessHandler;

        private List<string> m_characters = new();

        public string CharacterName { get; set; } = "Leeroy";

        public CharacterSelectionDialog(string name, DataAccessHandler dataAccessHandler)
        {
            m_accessHandler = dataAccessHandler;
            InitializeComponent();

            m_characters = dataAccessHandler.ListStoredEntities();
            CharacterName = inputName.Text = name;
            UpdateCharacterList();
        }

        private void UpdateCharacterList()
        {
            listBoxChars.Items.Clear();
            foreach (string name in m_characters)
            {
                listBoxChars.Items.Add(name);
            }
            if (m_characters.Any())
                listBoxChars.SelectedIndex = 0;
            btnSelect.Enabled = m_characters.Any();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            CharacterName = listBoxChars.Text;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            string name = inputName.Text;
            if (m_characters.Contains(name))
            {
                var result = MessageBox.Show("A character of this name already exists. Do you want to reset it?", 
                    "Careful now...", MessageBoxButtons.YesNo);
                if (result != DialogResult.Yes)
                {
                    return;
                }
                m_accessHandler.DeleteStoredEntity(name);
            }
            CharacterName = name;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void inputName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnCreate_Click(sender, e); // Incorrect sender does not matter here
            }
        }
    }
}
