﻿using OpenQA.Selenium;
using System;
using System.Windows;
using System.Threading.Tasks;

namespace GUI_VerificarMudancasNoQ
{
    public class Verificacao
    {
        private Driver_Login _login;
        public Driver_Login login_p { get => _login; }
        private int mudancas = 0;
        private string notas;
        private const string arquivo = "textodoq.log";
        private System.Media.SoundPlayer player = new System.Media.SoundPlayer(Properties.Resources.light);
        public MainWindow mainWindow { get; set; }

        public Verificacao(string login_janela, string senha_janela)
        {
            _login = new Driver_Login(login_janela, senha_janela);
        }

        public Verificacao(string login_janela, string senha_janela, MainWindow mainWin)
        {
            _login = new Driver_Login(login_janela, senha_janela);
            mainWindow = mainWin;
        }

        private void notificacao()
        {
            player.Play();
            mainWindow.notificacao_de_mudancas();
        }

        /*Função que compara um texto fornecido com o texto do arquivo, notifica e
        incrementa o contador de mudanças caso sejam diferentes e o arquivo não esteja em branco*/
        private void verificar_anterior(string comp1)
        {
            string comp2 = "";
            //Tenta ler o arquivo e o cria caso não exista
            try
            {
                comp2 = System.IO.File.ReadAllText(arquivo);
            }
            catch (System.IO.FileNotFoundException)
            {
                System.IO.File.Create(arquivo).Close();
            }
            //Comparação e notificação
            if (comp1 != comp2 && comp2 != "")
            {
                notificacao();
                mudancas++;
            }
        }

        public void verificar_acesso()
        {
            int numerneg = 1; //Index onde fica o texto obtido da página de acesso negado
            var elementos = _login.driver.FindElements(By.TagName("td"));
            //Caso se encontre na página de acesso negado, lança uma exceção de acesso negado
            if (elementos[numerneg].Text.Contains("Negado"))
            {
                throw new AcessoNegadoException("Verifique login e senha!");
            }
        }

        /*Função que obtem o texto da página configurada e escreve no arquivo o texto obtido*/
        public void verificar_texto()
        {
            int numerneg = 1; //Index onde fica o texto obtido da página de acesso negado
            int numer = 10; //Index onde geralmente fica o texto obtido da tag das páginas
            var elementos = _login.driver.FindElements(By.TagName("td"));
            //Caso se encontre na página de acesso negado, faz login novamente e retorna a função
            if (elementos[numerneg].Text.Contains("Negado"))
            {
                _login.fazer_login();
                return;
            }
            if (elementos.Count >= numer) { notas = elementos[numer].Text; }
            else { return; }
            notas = notas.Replace('\n', ' '); //Para melhor leitura do arquivo depois
            verificar_anterior(notas);
            //Salva o texto da página no arquivo e chama a função sleep_refresh
            System.IO.File.WriteAllText(arquivo, notas);
            _login.sleep_refresh();
        }

        //Inicia o driver e verifica o acesso à pagina
        public void iniciar_verificar_acesso()
        {
            _login.iniciar_driver();
            verificar_acesso();
        }
    }
}
