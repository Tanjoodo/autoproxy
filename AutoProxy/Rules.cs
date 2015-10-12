using System;

using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace AutoProxy
{
    static class Rules
    {
        static private ObservableCollection<ProxyRule> _rules = new ObservableCollection<ProxyRule>();
        
        static public void BindListView(ref ListView listView)
        {
            listView.ItemsSource = _rules;
        }

        static public void AddRule(ProxyRule rule)
        {
            if (rule == null) throw new ArgumentNullException("rule");
            if (rule.Default)
                foreach (var _rule in _rules)
                    if (_rule.Default)
                        throw new ArgumentException("A default rule already exists.");
            _rules.Add(rule);
        }

        static public void EditRule(ProxyRule sourceRule, ProxyRule destinationRule)
        {
            if (sourceRule == null) throw new ArgumentNullException("sourceRule");
            if (destinationRule == null) throw new ArgumentNullException("destinationRule");

            var source_index = _rules.IndexOf(sourceRule);
            if (source_index == -1) throw new ArgumentOutOfRangeException("sourceRule does not exist", "sourceRule");
            _rules[source_index] = destinationRule;
        }

        static public void RemoveRule(ProxyRule rule)
        {
            if (rule == null) throw new ArgumentNullException("rule");

            var rule_index = _rules.IndexOf(rule);
            if (rule_index == -1) throw new ArgumentException("rule does not exist", "rule");
            _rules.RemoveAt(rule_index);
        }
        
        static public ProxyRule FindRule(string ssid)
        {
            foreach (var rule in _rules)
                if (rule.SSID == ssid)
                    return rule;
            return null;
        }
        static public void LoadRules(string path)
        {
            FileStream istream;
            try
            {
                istream = new FileStream(path, FileMode.Open, FileAccess.Read);
            }
            catch
            {
                throw;
            }

            var formatter = new BinaryFormatter();
            _rules = (ObservableCollection<ProxyRule>)formatter.Deserialize(istream);
            istream.Close();
        }

        static public void WriteRules(string path)
        {
            FileStream ostream;

            try
            {
                ostream = new FileStream(path, FileMode.Create, FileAccess.Write);
            }
            catch
            {
                throw;
            }

            var formatter = new BinaryFormatter();
            formatter.Serialize(ostream, _rules);
            ostream.Close();
        }
    }
}
