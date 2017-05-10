using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlugSpl.Atlas;
using System.Windows.Controls;
using System.Windows;

namespace FeatureModelEditor {
    public interface IConstraintEditor {
        
        /// <summary>
        /// Update atlas inside constraint editor. It is called several times during program execution.
        /// </summary>
        void SetFeatureModel(AtlasFeatureModel model);
        
        /// <summary>
        /// Returns the editor itself.
        /// </summary>
        IConstraintEditor GetControl();

        /// <summary>
        /// Adds constraint to given AtlasFeatureModel. Returns 
        /// </summary>
        AtlasFeatureModel UpdateConstraintEditor(AtlasFeatureModel atlas);

        /// <summary>
        /// Adds an constraint to constraint editor.
        /// </summary>
        void AddConstraint(AtlasConstraint constraint);

       ListView contraintList();

        void Clear();
    }
}
