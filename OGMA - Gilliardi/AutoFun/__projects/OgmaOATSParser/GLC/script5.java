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
		beginStep("[1] No Title", 0);
		{
			web.textBox(
					2,
					"/web:window[@index='0' or @title='Google']/web:document[@index='0']/web:form[@id='gbqf' or @name='gbqf' or @index='0']/web:input_text[@id='gbqfq' or @name='q' or @index='0']")
					.setText("oracle");
			{
				think(2.166);
			}
		}
		endStep();
		beginStep("[2] No Title (/frame)", 0);
		{
			web.window(4, "/web:window[@index='0' or @title='Google']")
					.navigate(
							"https://plus.google.com/u/0/_/notifications/frame?sourceid=1&hl=pt-BR&hostiemode=11&origin=https%3A%2F%2Fwww.google.com.br&jsh=m%3B%2F_%2Fscs%2Fabc-static%2F_%2Fjs%2Fk%3Dgapi.gapi.en.2VNCBDQs37A.O%2Fm%3D__features__%2Frt%3Dj%2Fd%3D1%2Frs%3DAItRSTOlhbuorc8-HGwVGT4fcskZCcUb2A##pid=1&rpctoken=869408374&_methods=onError%2ConInfo%2ChideNotificationWidget%2CpostSharedMessage%2Creauth%2CsetNotificationWidgetHeight%2CsetNotificationWidgetSize%2CswitchTo%2CnavigateTo%2CsetNotificationText%2CsetNotificationAnimation%2CgetNotificationText%2C_ready%2C_close%2C_open%2C_resizeMe%2C_renderstart&id=I1_1413302669298&parent=https%3A%2F%2Fwww.google.com.br&pfname=");
			{
				think(0.696);
			}
			web.textBox(
					5,
					"/web:window[@index='0' or @title='Google']/web:document[@index='0']/web:form[@id='gbqf' or @name='gbqf' or @index='0']/web:input_text[@id='gbqfq' or @name='q' or @index='0']")
					.setText("oracle open script");
			{
				think(0.545);
			}
			web.textBox(
					6,
					"/web:window[@index='0' or @title='Google']/web:document[@index='0']/web:form[@id='gbqf' or @name='gbqf' or @index='0']/web:input_text[@id='gbqfq' or @name='q' or @index='0']")
					.pressEnter();
			{
				think(1.892);
			}
			web.link(
					7,
					"/web:window[@index='0' or @title='oracle open script - Pesquisa Google']/web:document[@index='0']/web:a[@text='Oracle Application Testing Suite Downloads' or @href='http://www.google.com.br/url?sa=t&rct=j&q=&esrc=s&source=web&cd=1&sqi=2&ved=0CB0QFjAA&url=http%3A%2F%2Fwww.oracle.com%2Ftechnetwork%2Foem%2Fdownloads%2Findex-084446.html&ei=kEk9VL7QBsfwgwSax4CIAg&usg=AFQjCNH00-4UhkUXkfboNvDhGo6gR1l2_w&bvm=bv.77161500,d.eXY' or @index='54']")
					.click();
			{
				think(3.847);
			}
			web.radioButton(
					8,
					"/web:window[@index='0' or @title='Oracle Application Testing Suite Downloads']/web:document[@index='0']/web:form[@name='agreementForm' or @index='1']/web:input_radio[(@name='agreement' and @value='on') or @index='0']")
					.select();
			{
				think(2.553);
			}
			web.link(
					9, 
					"/web:window[@index='0' or @title='Oracle Application Testing Suite Downloads']/web:document[@index='0']/web:a[@text='for Microsoft Windows (32-bit and 64-bit)' or @href='http://download.oracle.com/otn/nt/apptesting/12.4.0.2/oats-full-12.4.0.2.129.zip' or @index='611']")
					.click();
			{
				think(9.409);
			}
			web.textBox(
					10,
					"/web:window[@index='0' or @title='Entrada em Sessão Única – Entrada em Sessão']/web:document[@index='0']/web:form[@name='LoginForm' or @index='0']/web:input_text[@id='sso_username' or @name='ssousername' or @index='0']")
					.click();
			{
				think(0.766);
			}
			web.textBox(
					11,
					"/web:window[@index='0' or @title='Entrada em Sessão Única – Entrada em Sessão']/web:document[@index='0']/web:form[@name='LoginForm' or @index='0']/web:input_text[@id='sso_username' or @name='ssousername' or @index='0']")
					.setText("mau_oliveira@hotmail.com");
			{
				think(0.627);
			}
			web.textBox(
					12,
					"/web:window[@index='0' or @title='Entrada em Sessão Única – Entrada em Sessão']/web:document[@index='0']/web:form[@name='LoginForm' or @index='0']/web:input_text[@id='sso_username' or @name='ssousername' or @index='0']")
					.pressTab();
			{
				think(1.729);
			}
			web.textBox(
					13,
					"/web:window[@index='0' or @title='Entrada em Sessão Única – Entrada em Sessão']/web:document[@index='0']/web:form[@name='LoginForm' or @index='0']/web:input_password[@id='ssopassword' or @name='password' or @index='0']")
					.setPassword(deobfuscate("8Ki2N/CHLSP5bsQaUe25lQ=="));
			{
				think(0.946);
			}
			web.link(
					14,
					"/web:window[@index='0' or @title='Entrada em Sessão Única – Entrada em Sessão']/web:document[@index='0']/web:a[@text='Iniciar Sessão ' or @href='javascript:doLogin(document.LoginForm);' or @index='3']")
					.click();
			{
				think(9.258);
			}
			web.textBox(
					15,
					"/web:window[@index='0' or @title='Entrada em Sessão Única – Entrada em Sessão']/web:document[@index='0']/web:form[@name='LoginForm' or @index='0']/web:input_text[@id='sso_username' or @name='ssousername' or @index='0']")
					.click();
			{
				think(1.419);
			}
			web.textBox(
					16,
					"/web:window[@index='0' or @title='Entrada em Sessão Única – Entrada em Sessão']/web:document[@index='0']/web:form[@name='LoginForm' or @index='0']/web:input_text[@id='sso_username' or @name='ssousername' or @index='0']")
					.setText("mau_oliveira@hotmail.com");
			{
				think(0.127);
			}
			web.textBox(
					17,
					"/web:window[@index='0' or @title='Entrada em Sessão Única – Entrada em Sessão']/web:document[@index='0']/web:form[@name='LoginForm' or @index='0']/web:input_text[@id='sso_username' or @name='ssousername' or @index='0']")
					.pressTab();
			{
				think(2.26);
			}
			web.textBox(
					18,
					"/web:window[@index='0' or @title='Entrada em Sessão Única – Entrada em Sessão']/web:document[@index='0']/web:form[@name='LoginForm' or @index='0']/web:input_password[@id='ssopassword' or @name='password' or @index='0']")
					.setPassword(deobfuscate("NGDFyxjpWf60hLKu+M0jTQ=="));
			{
				think(1.062);
			}
			web.link(
					19,
					"/web:window[@index='0' or @title='Entrada em Sessão Única – Entrada em Sessão']/web:document[@index='0']/web:a[@text='Iniciar Sessão ' or @href='javascript:doLogin(document.LoginForm);' or @index='3']")
					.click();
			{
				think(17.457);
			}
			web.link(
					20,
					"/web:window[@index='0' or @title='Entrada em Sessão Única – Entrada em Sessão']/web:document[@index='0']/web:a[@text='Iniciar Sessão ' or @href='javascript:doLogin(document.LoginForm);' or @index='3']")
					.click();
		}
		endStep();

	}
	
	public void finish() throws Exception {
	}
}
