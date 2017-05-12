import oracle.oats.scripting.modules.basic.api.*;
import oracle.oats.scripting.modules.browser.api.*;
import oracle.oats.scripting.modules.functionalTest.api.*;
import oracle.oats.scripting.modules.utilities.api.*;
import oracle.oats.scripting.modules.utilities.api.sql.*;
import oracle.oats.scripting.modules.utilities.api.xml.*;
import oracle.oats.scripting.modules.utilities.api.file.*;
import oracle.oats.scripting.modules.webdom.api.*;

public class script extends IteratingVUserScript {
	@ScriptService oracle.oats.scripting.modules.utilities.api.UtilitiesService utilities;
	@ScriptService oracle.oats.scripting.modules.browser.api.BrowserService browser;
	@ScriptService oracle.oats.scripting.modules.functionalTest.api.FunctionalTestService ft;
	@ScriptService oracle.oats.scripting.modules.webdom.api.WebDomService web;
	
	public void initialize() throws Exception {
		browser.launch();
	}
		
	/**
	 * Add code to be executed each iteration for this virtual user.
	 */
	public void run() throws Exception {
		beginStep("[1] Formulários do Google: crie e analise pesquisas gratuitamente. (/ServiceLogin)",0);
		{
			web.window(2, "{{obj.GoogleDocsL.web_window_0}}").navigate("https://accounts.youtube.com/accounts/CheckConnection?pmpo=https%3A%2F%2Faccounts.google.com&v=-1202655622&timestamp=1414685482102");
			web.window(4, "{{obj.GoogleDocsL.web_window_0}}").waitForPage(null);
			{
				think(0.634);
			}
			web.element(6, "{{obj.GoogleDocsL.web_input_email_Email}}").click();
			{
				think(0.769);
			}
			web.textArea(7, "{{obj.GoogleDocsL.web_input_email_Email_1}}").setText("");
			{
				think(0.191);
			}
			web.textArea(8, "{{obj.GoogleDocsL.web_input_email_Email_2}}").setText("");
			{
				think(0.155);
			}
			web.textArea(9, "{{obj.GoogleDocsL.web_input_email_Email_3}}").setText("");
			{
				think(1.07);
			}
			web.textArea(10, "{{obj.GoogleDocsL.web_input_email_Email_4}}").setText("");
			{
				think(0.145);
			}
			web.textArea(11, "{{obj.GoogleDocsL.web_input_email_Email_5}}").setText("");
			{
				think(0.219);
			}
			web.textArea(12, "{{obj.GoogleDocsL.web_input_email_Email_6}}").setText("");
			{
				think(0.164);
			}
			web.textArea(13, "{{obj.GoogleDocsL.web_input_email_Email_7}}").setText("");
			{
				think(0.144);
			}
			web.textArea(14, "{{obj.GoogleDocsL.web_input_email_Email_8}}").setText("");
			{
				think(0.163);
			}
			web.textArea(15, "{{obj.GoogleDocsL.web_input_email_Email_9}}").setText("");
			{
				think(0.575);
			}
			web.textArea(16, "{{obj.GoogleDocsL.web_input_email_Email_10}}").setText("");
			{
				think(0.152);
			}
			web.textArea(17, "{{obj.GoogleDocsL.web_input_email_Email_8}}").setText("");
			{
				think(0.636);
			}
			web.textArea(18, "{{obj.GoogleDocsL.web_input_email_Email_11}}").setText("");
			{
				think(0.757);
			}
			web.textArea(19, "{{obj.GoogleDocsL.web_input_email_Email_12}}").setText("");
			{
				think(1.422);
			}
			web.textArea(20, "{{obj.GoogleDocsL.web_input_email_Email_13}}").setText("");
			{
				think(1.648);
			}
			web.textArea(21, "{{obj.GoogleDocsL.web_input_email_Email_14}}").setText("");
			{
				think(0.305);
			}
			web.textArea(22, "{{obj.GoogleDocsL.web_input_email_Email_15}}").setText("");
			{
				think(0.095);
			}
			web.textArea(23, "{{obj.GoogleDocsL.web_input_email_Email_16}}").setText("");
			{
				think(0.122);
			}
			web.textArea(24, "{{obj.GoogleDocsL.web_input_email_Email_17}}").setText("");
			{
				think(0.197);
			}
			web.textArea(25, "{{obj.GoogleDocsL.web_input_email_Email_18}}").setText("");
			{
				think(0.238);
			}
			web.textArea(26, "{{obj.GoogleDocsL.web_input_email_Email_19}}").setText("");
			{
				think(0.098);
			}
			web.textArea(27, "{{obj.GoogleDocsL.web_input_email_Email_20}}").setText("");
			{
				think(0.139);
			}
			web.textArea(28, "{{obj.GoogleDocsL.web_input_email_Email_21}}").setText("");
			{
				think(0.199);
			}
			web.textArea(29, "{{obj.GoogleDocsL.web_input_email_Email_22}}").setText("");
			{
				think(0.191);
			}
			web.element(30, "{{obj.GoogleDocsL.web_input_email_Email_22}}").pressTab();
			{
				think(1.107);
			}
			web.textBox(31, "{{obj.GoogleDocsL.web_input_password_Passwd}}").setPassword(deobfuscate("YdK7A2pckHAsmNRjElixFwjngmIUSRSp1YJxT3nXVdY="));
			{
				think(0.434);
			}
			web.textBox(32, "{{obj.GoogleDocsL.web_input_password_Passwd}}").pressEnter();
		}
		endStep();
		beginStep("[2] Formulário para teste do OATS (/viewform)", 0);
		{
			web.window(33, "{{obj.GoogleDocsL.web_window_0_1}}").waitForPage(null);
			{
				think(1.897);
			}
			web.textBox(34,"{{obj.GoogleDocsL.web_input_text_entry_987351091}}").click();
			{
				think(1.032);
			}
			web.textBox(35,"{{obj.GoogleDocsL.web_input_text_entry_987351091}}").setText("Juliana");
			{
				think(1.252);
			}
			web.textBox(36,	"{{obj.GoogleDocsL.web_input_text_entry_987351091}}").pressTab();
			{
				think(0.314);
			}
			web.textBox(37,	"{{obj.GoogleDocsL.web_input_text_entry_694305351}}").pressTab();
			{
				think(3.371);
			}
			web.textBox(38,	"{{obj.GoogleDocsL.web_input_text_entry_151186845}}").setText("27");
			{
				think(1.485);
			}
			web.textBox(39,	"{{obj.GoogleDocsL.web_input_text_entry_694305351}}").click();
			{
				think(0.696);
			}
			web.textBox(40,	"{{obj.GoogleDocsL.web_input_text_entry_694305351}}").setText("Damasio");
			{
				think(1.42);
			}
			web.radioButton(41,	"{{obj.GoogleDocsL.web_input_radio_group_1637076548_1}}").select();
			{
				think(1.174);
			}
			web.textArea(42, "{{obj.GoogleDocsL.web_textarea_entry_928988570}}").setText("adjasbdjaskbdabsdabhasbdfhsdbf\nadshjkhdakdaskL\n\n\nasdASA");
			{
				think(1.626);
			}
			web.checkBox(43,"{{obj.GoogleDocsL.web_input_checkbox_group_855054803_2}}").check(true);
			{
				think(0.556);
			}
			web.element(44, "{{obj.GoogleDocsL.web_li__Opção_3_}}").click();
			{
				think(0.437);
			}
			web.checkBox(45,"{{obj.GoogleDocsL.web_input_checkbox_group_855054803_4}}").check(true);
			{
				think(2.772);
			}
			web.selectBox(46, "{{obj.GoogleDocsL.web_select_entry_1343735785}}").selectOptionByText("Alegrete");
			{
				think(2.896);
			}
			web.selectBox(47, "{{obj.GoogleDocsL.web_select_entry_1343735785}}").selectOptionByText("Porto Alegre");
			{
				think(2.029);
			}
			web.radioButton(48,"{{obj.GoogleDocsL.web_input_radio_group_1006956115_3}}").select();
			{
				think(2.608);
			}
			web.radioButton(49,"{{obj.GoogleDocsL.web_input_radio_group_1006956115_5}}").select();
			{
				think(2.0);
			}
			web.radioButton(50,"{{obj.GoogleDocsL.web_input_radio_group_1818246776_1}}").select();
			{
				think(0.771);
			}
			web.radioButton(51,"{{obj.GoogleDocsL.web_input_radio_group_1796021045_2}}").select();
			{
				think(4.833);
			}
			web.selectBox(52,"{{obj.GoogleDocsL.web_select_entry_1412311749_month}}").selectOptionByText("Junho");
			{
				think(2.005);
			}
			web.selectBox(53,"{{obj.GoogleDocsL.web_select_entry_1412311749_day}}").selectOptionByText("18");
			{
				think(4.83);
			}
			web.selectBox(54,"{{obj.GoogleDocsL.web_select_entry_1412311749_year}}").selectOptionByText("1987");
			{
				think(4.714);
			}
			web.selectBox(55,"{{obj.GoogleDocsL.web_select_entry_1542190611_hour}}").selectOptionByText("14");
			{
				think(2.683);
			}
			web.selectBox(56,"{{obj.GoogleDocsL.web_select_entry_1542190611_minute}}").selectOptionByText("12");
			{
				think(0.537);
			}
			web.button(57, "{{obj.GoogleDocsL.web_input_submit_ss_submit}}").click();
		}
		endStep();
		beginStep("[3] Obrigado! (/formResponse)", 0);
		{
			web.window(58, "{{obj.GoogleDocsL.web_window_0_2}}").waitForPage(null);
		}
		endStep();
	}
	
	public void finish() throws Exception {
	}
}
