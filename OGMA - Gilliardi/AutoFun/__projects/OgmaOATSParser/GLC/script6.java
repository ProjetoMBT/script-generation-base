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
		beginStep("[1] No Title (/PT-BR.html)", 0);
		{
			web.window(2, "/web:window[@index='0' or @title='Entrar']")
					.navigate(
							"https://sc.imp.live.com/content/dam/imp/surfaces/mail_signin/header/mail/PT-BR.html?id=64855&mkt=PT-BR&cbcxt=mai");
			{
				think(2.265);
			}
			web.textArea(
					4,
					"/web:window[@index='0' or @title='Entrar']/web:document[@index='0']/web:form[@name='f1' or @index='0']/web:input_email[@id='i0116' and @className='ltr_override' and @lang='en' and @aria-labelledby='idLbl_PWD_Username' and @maxlength='113' and @name='login' and @checked='False' and @disabled='False' and @readonly='False' and @size='20' and @tabindex='0' and @value='mau' and @defaultChecked='False' and @height='0' and @width='0' and @hspace='0' and @index='0']")
					.setText("");
			{
				think(0.812);
			}
			web.textArea(
					5,
					"/web:window[@index='0' or @title='Entrar']/web:document[@index='0']/web:form[@name='f1' or @index='0']/web:input_email[@id='i0116' and @className='ltr_override' and @lang='en' and @aria-labelledby='idLbl_PWD_Username' and @maxlength='113' and @name='login' and @checked='False' and @disabled='False' and @readonly='False' and @size='20' and @tabindex='0' and @value='mau_' and @defaultChecked='False' and @height='0' and @width='0' and @hspace='0' and @index='0']")
					.setText("");
			{
				think(0.024);
			}
			web.textArea(
					6,
					"/web:window[@index='0' or @title='Entrar']/web:document[@index='0']/web:form[@name='f1' or @index='0']/web:input_email[@id='i0116' and @className='ltr_override' and @lang='en' and @aria-labelledby='idLbl_PWD_Username' and @maxlength='113' and @name='login' and @checked='False' and @disabled='False' and @readonly='False' and @size='20' and @tabindex='0' and @value='mau_o' and @defaultChecked='False' and @height='0' and @width='0' and @hspace='0' and @index='0']")
					.setText("");
			{
				think(0.133);
			}
			web.textArea(
					7,
					"/web:window[@index='0' or @title='Entrar']/web:document[@index='0']/web:form[@name='f1' or @index='0']/web:input_email[@id='i0116' and @className='ltr_override' and @lang='en' and @aria-labelledby='idLbl_PWD_Username' and @maxlength='113' and @name='login' and @checked='False' and @disabled='False' and @readonly='False' and @size='20' and @tabindex='0' and @value='mau_ol' and @defaultChecked='False' and @height='0' and @width='0' and @hspace='0' and @index='0']")
					.setText("");
			{
				think(0.164);
			}
			web.textArea(
					8,
					"/web:window[@index='0' or @title='Entrar']/web:document[@index='0']/web:form[@name='f1' or @index='0']/web:input_email[@id='i0116' and @className='ltr_override' and @lang='en' and @aria-labelledby='idLbl_PWD_Username' and @maxlength='113' and @name='login' and @checked='False' and @disabled='False' and @readonly='False' and @size='20' and @tabindex='0' and @value='mau_oli' and @defaultChecked='False' and @height='0' and @width='0' and @hspace='0' and @index='0']")
					.setText("");
			{
				think(0.212);
			}
			web.textArea(
					9,
					"/web:window[@index='0' or @title='Entrar']/web:document[@index='0']/web:form[@name='f1' or @index='0']/web:input_email[@id='i0116' and @className='ltr_override' and @lang='en' and @aria-labelledby='idLbl_PWD_Username' and @maxlength='113' and @name='login' and @checked='False' and @disabled='False' and @readonly='False' and @size='20' and @tabindex='0' and @value='mau_oliv' and @defaultChecked='False' and @height='0' and @width='0' and @hspace='0' and @index='0']")
					.setText("");
			{
				think(0.039);
			}
			web.textArea(
					10,
					"/web:window[@index='0' or @title='Entrar']/web:document[@index='0']/web:form[@name='f1' or @index='0']/web:input_email[@id='i0116' and @className='ltr_override' and @lang='en' and @aria-labelledby='idLbl_PWD_Username' and @maxlength='113' and @name='login' and @checked='False' and @disabled='False' and @readonly='False' and @size='20' and @tabindex='0' and @value='mau_olive' and @defaultChecked='False' and @height='0' and @width='0' and @hspace='0' and @index='0']")
					.setText("");
			{
				think(0.133);
			}
			web.textArea(
					11,
					"/web:window[@index='0' or @title='Entrar']/web:document[@index='0']/web:form[@name='f1' or @index='0']/web:input_email[@id='i0116' and @className='ltr_override' and @lang='en' and @aria-labelledby='idLbl_PWD_Username' and @maxlength='113' and @name='login' and @checked='False' and @disabled='False' and @readonly='False' and @size='20' and @tabindex='0' and @value='mau_olivei' and @defaultChecked='False' and @height='0' and @width='0' and @hspace='0' and @index='0']")
					.setText("");
			{
				think(0.225);
			}
			web.textArea(
					12,
					"/web:window[@index='0' or @title='Entrar']/web:document[@index='0']/web:form[@name='f1' or @index='0']/web:input_email[@id='i0116' and @className='ltr_override' and @lang='en' and @aria-labelledby='idLbl_PWD_Username' and @maxlength='113' and @name='login' and @checked='False' and @disabled='False' and @readonly='False' and @size='20' and @tabindex='0' and @value='mau_oliveir' and @defaultChecked='False' and @height='0' and @width='0' and @hspace='0' and @index='0']")
					.setText("");
			{
				think(0.042);
			}
			web.textArea(
					13,
					"/web:window[@index='0' or @title='Entrar']/web:document[@index='0']/web:form[@name='f1' or @index='0']/web:input_email[@id='i0116' and @className='ltr_override' and @lang='en' and @aria-labelledby='idLbl_PWD_Username' and @maxlength='113' and @name='login' and @checked='False' and @disabled='False' and @readonly='False' and @size='20' and @tabindex='0' and @value='mau_oliveira' and @defaultChecked='False' and @height='0' and @width='0' and @hspace='0' and @index='0']")
					.setText("");
			{
				think(0.31);
			}
			web.textArea(
					14,
					"/web:window[@index='0' or @title='Entrar']/web:document[@index='0']/web:form[@name='f1' or @index='0']/web:input_email[@id='i0116' and @className='ltr_override' and @lang='en' and @aria-labelledby='idLbl_PWD_Username' and @maxlength='113' and @name='login' and @checked='False' and @disabled='False' and @readonly='False' and @size='20' and @tabindex='0' and @value='mau_oliveira@' and @defaultChecked='False' and @height='0' and @width='0' and @hspace='0' and @index='0']")
					.setText("");
			{
				think(0.699);
			}
			web.textArea(
					15,
					"/web:window[@index='0' or @title='Entrar']/web:document[@index='0']/web:form[@name='f1' or @index='0']/web:input_email[@id='i0116' and @className='ltr_override' and @lang='en' and @aria-labelledby='idLbl_PWD_Username' and @maxlength='113' and @name='login' and @checked='False' and @disabled='False' and @readonly='False' and @size='20' and @tabindex='0' and @value='mau_oliveira@h' and @defaultChecked='False' and @height='0' and @width='0' and @hspace='0' and @index='0']")
					.setText("");
			{
				think(0.409);
			}
			web.textArea(
					16,
					"/web:window[@index='0' or @title='Entrar']/web:document[@index='0']/web:form[@name='f1' or @index='0']/web:input_email[@id='i0116' and @className='ltr_override' and @lang='en' and @aria-labelledby='idLbl_PWD_Username' and @maxlength='113' and @name='login' and @checked='False' and @disabled='False' and @readonly='False' and @size='20' and @tabindex='0' and @value='mau_oliveira@ho' and @defaultChecked='False' and @height='0' and @width='0' and @hspace='0' and @index='0']")
					.setText("");
			{
				think(0.084);
			}
			web.textArea(
					17,
					"/web:window[@index='0' or @title='Entrar']/web:document[@index='0']/web:form[@name='f1' or @index='0']/web:input_email[@id='i0116' and @className='ltr_override' and @lang='en' and @aria-labelledby='idLbl_PWD_Username' and @maxlength='113' and @name='login' and @checked='False' and @disabled='False' and @readonly='False' and @size='20' and @tabindex='0' and @value='mau_oliveira@hot' and @defaultChecked='False' and @height='0' and @width='0' and @hspace='0' and @index='0']")
					.setText("");
			{
				think(0.171);
			}
			web.textArea(
					18,
					"/web:window[@index='0' or @title='Entrar']/web:document[@index='0']/web:form[@name='f1' or @index='0']/web:input_email[@id='i0116' and @className='ltr_override' and @lang='en' and @aria-labelledby='idLbl_PWD_Username' and @maxlength='113' and @name='login' and @checked='False' and @disabled='False' and @readonly='False' and @size='20' and @tabindex='0' and @value='mau_oliveira@hotm' and @defaultChecked='False' and @height='0' and @width='0' and @hspace='0' and @index='0']")
					.setText("");
			{
				think(0.071);
			}
			web.textArea(
					19,
					"/web:window[@index='0' or @title='Entrar']/web:document[@index='0']/web:form[@name='f1' or @index='0']/web:input_email[@id='i0116' and @className='ltr_override' and @lang='en' and @aria-labelledby='idLbl_PWD_Username' and @maxlength='113' and @name='login' and @checked='False' and @disabled='False' and @readonly='False' and @size='20' and @tabindex='0' and @value='mau_oliveira@hotma' and @defaultChecked='False' and @height='0' and @width='0' and @hspace='0' and @index='0']")
					.setText("");
			{
				think(0.037);
			}
			web.textArea(
					20,
					"/web:window[@index='0' or @title='Entrar']/web:document[@index='0']/web:form[@name='f1' or @index='0']/web:input_email[@id='i0116' and @className='ltr_override' and @lang='en' and @aria-labelledby='idLbl_PWD_Username' and @maxlength='113' and @name='login' and @checked='False' and @disabled='False' and @readonly='False' and @size='20' and @tabindex='0' and @value='mau_oliveira@hotmai' and @defaultChecked='False' and @height='0' and @width='0' and @hspace='0' and @index='0']")
					.setText("");
			{
				think(0.186);
			}
			web.textArea(
					21,
					"/web:window[@index='0' or @title='Entrar']/web:document[@index='0']/web:form[@name='f1' or @index='0']/web:input_email[@id='i0116' and @className='ltr_override' and @lang='en' and @aria-labelledby='idLbl_PWD_Username' and @maxlength='113' and @name='login' and @checked='False' and @disabled='False' and @readonly='False' and @size='20' and @tabindex='0' and @value='mau_oliveira@hotmail' and @defaultChecked='False' and @height='0' and @width='0' and @hspace='0' and @index='0']")
					.setText("");
			{
				think(0.207);
			}
			web.textArea(
					22,
					"/web:window[@index='0' or @title='Entrar']/web:document[@index='0']/web:form[@name='f1' or @index='0']/web:input_email[@id='i0116' and @className='ltr_override' and @lang='en' and @aria-labelledby='idLbl_PWD_Username' and @maxlength='113' and @name='login' and @checked='False' and @disabled='False' and @readonly='False' and @size='20' and @tabindex='0' and @value='mau_oliveira@hotmail.' and @defaultChecked='False' and @height='0' and @width='0' and @hspace='0' and @index='0']")
					.setText("");
			{
				think(0.102);
			}
			web.textArea(
					23,
					"/web:window[@index='0' or @title='Entrar']/web:document[@index='0']/web:form[@name='f1' or @index='0']/web:input_email[@id='i0116' and @className='ltr_override' and @lang='en' and @aria-labelledby='idLbl_PWD_Username' and @maxlength='113' and @name='login' and @checked='False' and @disabled='False' and @readonly='False' and @size='20' and @tabindex='0' and @value='mau_oliveira@hotmail.c' and @defaultChecked='False' and @height='0' and @width='0' and @hspace='0' and @index='0']")
					.setText("");
			{
				think(0.178);
			}
			web.textArea(
					24,
					"/web:window[@index='0' or @title='Entrar']/web:document[@index='0']/web:form[@name='f1' or @index='0']/web:input_email[@id='i0116' and @className='ltr_override' and @lang='en' and @aria-labelledby='idLbl_PWD_Username' and @maxlength='113' and @name='login' and @checked='False' and @disabled='False' and @readonly='False' and @size='20' and @tabindex='0' and @value='mau_oliveira@hotmail.com' and @defaultChecked='False' and @height='0' and @width='0' and @hspace='0' and @index='0']")
					.setText("");
			{
				think(0.065);
			}
			web.element(
					25,
					"/web:window[@index='0' or @title='Entrar']/web:document[@index='0']/web:form[@name='f1' or @index='0']/web:input_email[@id='i0116' and @className='ltr_override' and @lang='en' and @aria-labelledby='idLbl_PWD_Username' and @maxlength='113' and @name='login' and @checked='False' and @disabled='False' and @readonly='False' and @size='20' and @tabindex='0' and @value='mau_oliveira@hotmail.com' and @defaultChecked='False' and @height='0' and @width='0' and @hspace='0' and @index='0']")
					.pressTab();
			{
				think(0.216);
			}
			web.textBox(
					26,
					"/web:window[@index='0' or @title='Entrar']/web:document[@index='0']/web:form[@name='f1' or @index='0']/web:input_password[@id='i0118' or @name='passwd' or @index='0']")
					.setPassword(deobfuscate("Vo+ul2zSg5AqYRB2ktQgWQ=="));
			{
				think(2.029);
			}
			web.checkBox(
					27,
					"/web:window[@index='0' or @title='Entrar']/web:document[@index='0']/web:form[@name='f1' or @index='0']/web:input_checkbox[@id='idChkBx_PWD_KMSI0Pwd' or @name='KMSI' or @index='0']")
					.check(true);
			{
				think(2.453);
			}
			web.button(
					28,
					"/web:window[@index='0' or @title='Entrar']/web:document[@index='0']/web:form[@name='f1' or @index='0']/web:input_submit[@id='idSIButton9' or @name='SI' or @value='Entrar' or @index='0']")
					.click();
			{
				think(8.663);
			}
			web.element(
					29,
					"/web:window[@index='0' or @title='Você solicitou a alteração das informações de segurança?']/web:document[@index='0']/web:span[@text='Maurício Oliveira' or @id='c_meun' or @index='7']")
					.click();
			{
				think(1.231);
			}
			web.link(
					30,
					"/web:window[@index='0' or @title='Você solicitou a alteração das informações de segurança?']/web:document[@index='0']/web:a[@text='Sair' or @href='https://login.live.com/logout.srf?ct=1414153915&rver=6.5.6510.0&lc=1046&id=38936&ru=https:%2F%2Flogin.live.com%2Flogin.srf%3Flc%3D1046%26sf%3D1%26id%3D64855%26tw%3D18000%26fs%3D0%26ts%3D-1%26cbcxt%3Dmai%26sec%3D%26mspp_shared%3D1%26seclog%3D10%26wp%3DMBI_SSL_SHARED%26ru%3Dhttps:%2F%2Fmail.live.com%2Fdefault.aspx%253frru%253dinbox%26scft%3DClyf4FT!Fom1eOOllgY9q7uEzePlXDgZI9Spn!UeETrYwiMH3lzraB6RYvaj54zXxrhhSw0MuqpzQd1ki*ApLyzljLC4NRYPVBre4u5G4q3F5Cd1x89pW7ik6ta8KG9wsaQpiOmR4JZzOpavfGguHNsMrtIK3dbAHchDKWl4sShA&mkt=pt-BR&mkt=pt-BR' or @index='3']")
					.click();
		}
		endStep();

	}
	
	public void finish() throws Exception {
	}
}
