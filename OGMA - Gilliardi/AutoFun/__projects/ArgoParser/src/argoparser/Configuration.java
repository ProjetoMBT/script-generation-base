/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

package argoparser;

import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.util.Properties;
import java.util.logging.Level;
import java.util.logging.Logger;

/**
 *
 * @author Cristiano M Martins
 * @since Jul 15, 2015 5:40:44 PM
 */
public class Configuration {
    
    private Properties properties;
    private static Configuration instance;
    
    
    private Configuration(){
        try {
            this.properties = new Properties();
            properties.load(new FileInputStream(new File("/Config.cfg")));
        } catch (IOException ex) {
            Logger.getLogger(Configuration.class.getName()).log(Level.SEVERE, null, ex);
        }
    }
    
    public static synchronized Configuration getInstance(){
        if(instance == null){
            instance = new Configuration();
        }
        return instance;
    }
    
    public String getProperty(String propertyName){
        return (String)properties.get(propertyName);
    }
    
}
