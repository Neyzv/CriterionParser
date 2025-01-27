using CriterionParser.Enums;
using CriterionParser.Models;

namespace CriterionParser.Factories;

public interface ICriteriaFactory
{
    Criteria Create(string idenfitier, Operator op, Comparator comparator, string value); 
}