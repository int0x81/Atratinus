namespace Atratinus.Core.Enums
{
    public enum PurposeOfTransactionType
    {
        /// <summary>
        /// deal with access cash, underlevarage, dividends/ repurchases, stop/ reduce equity issuance, restructuing debt, recapitalization
        /// </summary>
        CHANGE_CAPITAL_STRUCTURE,
        /// <summary>
        /// achieve operational efficiency, achieve strategic partnership, obtain focus (business restructuring and spinning off)
        /// </summary>
        ALTER_BUSINESS_STRATEGY,
        /// <summary>
        /// M&A already happening: carry out M&A: as target (against a deal/ for better terms), carry out M&A: as acquirer (against a deal/ for better terms) Close M&A deal
        /// </summary>
        M_AND_A,
        /// <summary>
        /// sell target company or main asset to third party, take control of/ buy out company and/ or take it private
        /// </summary>
        SELL_TARGET_COMPANY,
        /// <summary>
        /// rescind takeover defenses; oust CEO, board chair; achieve board independence and fair representation; 
        /// obtain more informations dicslosure/potential fraud; alter excess executive compensation/ pay for performance
        /// </summary>
        IMPROVE_GOVERNANCE,
        BANKRUPTCY,
        /// <summary>
        /// company generally undervaluedm simple investment purposes
        /// </summary>
        OTHER,
        TRADING_STRATEGY_BY_ACTIVIST
    }
}
