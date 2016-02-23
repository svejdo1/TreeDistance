//    Copyright (C) 2012  Mateusz Pawlik and Nikolaus Augsten
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU Affero General Public License as
//    published by the Free Software Foundation, either version 3 of the
//    License, or (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU Affero General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System.Collections.Generic;

namespace Barbar.TreeDistance.Util {

    /**
     * This provides a way of using small int values to represent string labels, 
     * as opposed to storing the labels directly.
     * 
     * @author Denilson Barbosa, Nikolaus Augsten  from approxlib, available at http://www.inf.unibz.it/~augsten/src/
     */
    public class LabelDictionary {
        public const int KEY_DUMMY_LABEL = -1;
        private int count;
        private IDictionary<string, int> StrInt;
        private IDictionary<int, string> IntStr;
        private bool newLabelsAllowed = true;

        /**
         * Creates a new blank dictionary.
         *  
         * @throws Exception
         */
        public LabelDictionary() {
            count = 0;
            StrInt = new Dictionary<string, int>();
            IntStr = new Dictionary<int, string>();
        }


        /**
         * Adds a new label to the dictionary if it has not been added yet. 
         * Returns the ID of the new label in the dictionary.
         * 
         * @param label add this label to the dictionary if it does not exist yet
         * @return ID of label in the dictionary
         */
        public int store(string label) {
            if (StrInt.ContainsKey(label)) {
                return (StrInt[label]);
            } else if (!newLabelsAllowed) {
                return KEY_DUMMY_LABEL;
            } else { // store label
                var intKey = count++;
                StrInt.Add(label, intKey);
                IntStr.Add(intKey, label);

                return intKey;
            }
        }

        /**
         * Returns the label with a given ID in the dictionary.
         *	
         * @param labelID 
         * @return the label with the specified labelID, or null if this dictionary contains no label for labelID
         */
        public string read(int labelID) {
            return IntStr[labelID];
        }

        /**
         * @return true iff new labels can be stored into this label dictinoary
         */
        public bool isNewLabelsAllowed() {
            return newLabelsAllowed;
        }

        /**
         * @param newLabelsAllowed the newLabelsAllowed to set
         */
        public void setNewLabelsAllowed(bool newLabelsAllowed) {
            this.newLabelsAllowed = newLabelsAllowed;
        }
    }
}