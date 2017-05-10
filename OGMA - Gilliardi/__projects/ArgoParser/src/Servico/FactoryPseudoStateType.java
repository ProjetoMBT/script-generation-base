/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package Servico;

import Objects.UmlDecision;
import Objects.UmlFork;
import Objects.UmlInitialState;
import Objects.UmlJoin;
import Objects.UmlPseudoState;

/**
 *
 * @author Administrator
 */
public class FactoryPseudoStateType
{
    public UmlPseudoState methodFactoryPseudoStateType(String type)
    {
        switch (type)
        {
            case "initial":
                return new UmlInitialState();
            case "fork":
                return new UmlFork();
            case "join":
                return new UmlJoin();
            case "decision":
                return new UmlDecision();
        }
        return null;
    }
}
