namespace CompiladorComInterface
{
    public partial class Compilador : Form
    {
        public Compilador()
        {
            InitializeComponent();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnAnalisar_Click(object sender, EventArgs e)
        {
            string codigo = txtCodigo.Text;
            var tokens = AnalisadorLexico.Analisar(codigo);

            dgvTokens.Rows.Clear();
            lstErros.Items.Clear();

            foreach (var token in tokens)
            {
                dgvTokens.Rows.Add(token.Codigo, token.Lexema, token.Linha);
            }

            foreach (var erro in AnalisadorLexico.Erros)
            {
                lstErros.Items.Add(erro);
            }
        }
    }
}
