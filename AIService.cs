using Hamekaraa.Model.ViewModel.Order;
using Hamekaraa.Service.Tools;
using System.Data;

namespace Hamekaraa.Service.Services
{
    public interface IAIService
    {
        //List<AssociationRuleVM> FillAssociationRulesVM(bool IsSucces);
        List<AprioriResultVM> MineAssociationRules(List<List<string>> transactions, int minSupport = 50, double minConfidence = 0.5);
    }

    public class AIService : IAIService, IScoped
    {

        public List<AprioriResultVM> MineAssociationRules(List<List<string>> transactions, int minSupport = 10, double minConfidence = 0.5)
        {
            var frequentItemSets = GenerateFrequentItemSets(transactions, minSupport);

            var associationRules = GenerateAssociationRules(frequentItemSets, transactions, minConfidence);

            return associationRules.Select(x => new AprioriResultVM()
            {
                Confidence = x.Item3,
                rule = $"{string.Join(", ", x.Item1)} => {string.Join(", ", x.Item2)}"
            }).ToList();
        }

        private List<List<string>> GenerateFrequentItemSets(List<List<string>> transactions, int minSupport)
        {
            var uniqueItems = transactions.SelectMany(t => t).Distinct().ToList();
            var frequentItemSets = new List<List<string>>();

            foreach (var item in uniqueItems)
            {
                var support = transactions.Count(t => t.Contains(item));
                if (support >= minSupport)
                {
                    frequentItemSets.Add(new List<string> { item });
                }
            }

            int k = 2;
            while (true)
            {
                var candidateItemSets = GenerateCandidateItemSets(frequentItemSets, k, transactions, minSupport);

                if (candidateItemSets.Count == 0)
                    break;

                frequentItemSets.AddRange(candidateItemSets);
                k++;
            }

            return frequentItemSets;
        }

        private List<List<string>> GenerateCandidateItemSets(List<List<string>> frequentItemSets, int k, List<List<string>> transactions, int minSupport)
        {
            var candidates = new List<List<string>>();

            foreach (var set1 in frequentItemSets)
            {
                foreach (var set2 in frequentItemSets)
                {
                    var joinedSet = set1.Concat(set2).Distinct().OrderBy(item => item).ToList();

                    if (joinedSet.Count == k && !candidates.Any(c => c.SequenceEqual(joinedSet)))
                    {
                        candidates.Add(joinedSet);
                    }
                }
            }

            return candidates.Where(candidateSet => CountSupport(candidateSet, transactions) >= minSupport).ToList();
        }

        private double CountSupport(List<string> itemSet, List<List<string>> transactions)
        {
            return transactions.Count(t => itemSet.All(item => t.Contains(item)));
        }

        private List<Tuple<List<string>, List<string>, double>> GenerateAssociationRules(List<List<string>> frequentItemSets, List<List<string>> transactions, double minConfidence)
        {
            var associationRules = new List<Tuple<List<string>, List<string>, double>>();

            foreach (var itemSet in frequentItemSets)
            {
                if (itemSet.Count >= 2)
                {
                    GenerateRulesRecursive(itemSet, itemSet, associationRules, transactions, minConfidence);
                }
            }

            return associationRules;
        }

        private void GenerateRulesRecursive(List<string> originalSet, List<string> currentSet, List<Tuple<List<string>, List<string>, double>> associationRules, List<List<string>> transactions, double minConfidence)
        {
            for (int i = 0; i < currentSet.Count; i++)
            {
                var consequent = new List<string> { currentSet[i] };
                var antecedent = currentSet.Except(consequent).ToList();

                double confidence = CountSupport(originalSet, transactions) / CountSupport(antecedent, transactions);

                if (confidence >= minConfidence)
                {
                    associationRules.Add(new Tuple<List<string>, List<string>, double>(antecedent, consequent, confidence));
                }

                if (antecedent.Count >= 2)
                {
                    GenerateRulesRecursive(originalSet, antecedent, associationRules, transactions, minConfidence);
                }
            }
        }
    }
}

