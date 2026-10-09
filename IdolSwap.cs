using System.Collections.Generic;

namespace NhxIdolProgression
{
    // Puts the idol an upgrade takes (see Idols.IdolForUpgrade) into the recipe's idol requirement while the game, or
    // another mod, checks, shows or takes the requirements of that one upgrade, and puts the recipe's own idol back
    // afterwards. A recipe is shared by every copy of its item, whatever the copy's level, so the requirement can't be
    // changed for good: Reforged Potential did, which made the Forge of Potential check every item in its list against
    // the idol of the last item selected.
    internal sealed class IdolSwap
    {
        private sealed class Swapped
        {
            internal ItemDrop RecipeIdol;
            internal int Depth;
        }

        // Requirements currently holding another idol, with the idol their recipe really asks for. Swaps nest (crafting
        // checks the requirements again inside the crafting panel's update), so a requirement only gets its recipe's idol
        // back when its last swap ends.
        private static readonly Dictionary<Piece.Requirement, Swapped> s_swapped = new Dictionary<Piece.Requirement, Swapped>();

        private readonly List<KeyValuePair<Piece.Requirement, ItemDrop>> _previous = new List<KeyValuePair<Piece.Requirement, ItemDrop>>();

        private IdolSwap()
        {
        }

        // Returns null when nothing had to change, e.g. the upgrade still takes the recipe's own idol
        internal static IdolSwap Begin(Recipe recipe, int targetQuality)
        {
            if (recipe == null || recipe.m_resources == null)
            {
                return null;
            }

            IdolSwap swap = null;

            foreach (Piece.Requirement requirement in recipe.m_resources)
            {
                if (requirement == null || !requirement.m_upgraderResource || requirement.m_resItem == null)
                {
                    continue;
                }

                s_swapped.TryGetValue(requirement, out Swapped swapped);
                ItemDrop recipeIdol = swapped != null ? swapped.RecipeIdol : requirement.m_resItem;
                ItemDrop idol = Idols.IdolForUpgrade(recipeIdol, targetQuality);

                if (idol == null || idol == requirement.m_resItem)
                {
                    continue;
                }

                if (swapped == null)
                {
                    swapped = new Swapped { RecipeIdol = recipeIdol };
                    s_swapped[requirement] = swapped;
                }

                swapped.Depth++;

                if (swap == null)
                {
                    swap = new IdolSwap();
                }

                swap._previous.Add(new KeyValuePair<Piece.Requirement, ItemDrop>(requirement, requirement.m_resItem));
                requirement.m_resItem = idol;
            }

            return swap;
        }

        internal void End()
        {
            for (int i = _previous.Count - 1; i >= 0; i--)
            {
                Piece.Requirement requirement = _previous[i].Key;
                requirement.m_resItem = _previous[i].Value;

                if (s_swapped.TryGetValue(requirement, out Swapped swapped) && --swapped.Depth <= 0)
                {
                    s_swapped.Remove(requirement);
                }
            }

            _previous.Clear();
        }
    }
}
