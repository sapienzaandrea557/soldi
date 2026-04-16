using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using System;
using System.Collections.Generic;
using System.Linq;

namespace cAlgo.Robots
{
    [Robot(AccessRights = AccessRights.FullAccess, TimeZone = TimeZones.UTC)]
    public class VITPredittore_v2_2 : Robot
    {
        // ============================================================
        // 1. PARAMETRI CONFIGURABILI [Parameter]
        // ============================================================

        // --- Scan ---
        [Parameter("History Depth", DefaultValue = 15000, MinValue = 100)]
        public int HistoryDepth { get; set; }

        [Parameter("Analysis Window", DefaultValue = 8, MinValue = 2)]
        public int AnalysisWindow { get; set; }

        [Parameter("Similarity Threshold", DefaultValue = 65, MinValue = 0, MaxValue = 100)]
        public double SimilarityThreshold { get; set; }

        [Parameter("Aftermath Threshold", DefaultValue = 58, MinValue = 0, MaxValue = 100)]
        public double AftermathThreshold { get; set; }

        [Parameter("Aftermath Top N", DefaultValue = 50, MinValue = 1)]
        public int AftermathTopN { get; set; }

        [Parameter("Survival Threshold", DefaultValue = 55, MinValue = 0, MaxValue = 100)]
        public double SurvivalThreshold { get; set; }

        [Parameter("Default Ghost Count", DefaultValue = 10, MinValue = 1)]
        public int DefaultGhostCount { get; set; }

        [Parameter("Refresh Seconds", DefaultValue = 15)]
        public int RefreshSeconds { get; set; }

        // --- Conferma Storica ---
        [Parameter("Movement ATR Multiplier", DefaultValue = 1.5)]
        public double MovementATRMultiplier { get; set; }

        [Parameter("Aftermath Candles", DefaultValue = 10, MinValue = 1)]
        public int AftermathCandles { get; set; }

        [Parameter("Min Occurrences", DefaultValue = 3, MinValue = 1)]
        public int MinOccurrences { get; set; }

        // --- Filtri ---
        [Parameter("Decay Factor", DefaultValue = 0.95, MinValue = 0.5, MaxValue = 1.0)]
        public double DecayFactor { get; set; }

        [Parameter("Clone Distance", DefaultValue = 5, MinValue = 1)]
        public int CloneDistance { get; set; }

        [Parameter("Clarity Threshold", DefaultValue = 0.3)]
        public double ClarityThreshold { get; set; }

        [Parameter("Cancellation Diff", DefaultValue = 3)]
        public int CancellationDiff { get; set; }

        [Parameter("Cancellation Ratio", DefaultValue = 0.20)]
        public double CancellationRatio { get; set; }

        [Parameter("Deviation Threshold", DefaultValue = 1.5)]
        public double DeviationThreshold { get; set; }

        [Parameter("Bonus Pattern Library", DefaultValue = 5)]
        public double BonusPatternLibrary { get; set; }

        [Parameter("Similarity Sigma", DefaultValue = 0.25, MinValue = 0.01)]
        public double SimilaritySigma { get; set; }

        [Parameter("Min Signal Confidence", DefaultValue = 65, MinValue = 0, MaxValue = 100)]
        public double MinSignalConfidence { get; set; }

        [Parameter("Inertia Penalty", DefaultValue = 5.0)]
        public double InertiaPenalty { get; set; }

        [Parameter("Volatility Sensitivity", DefaultValue = 0.5, MinValue = 0.0, MaxValue = 1.0)]
        public double VolatilitySensitivity { get; set; }

        [Parameter("Min Signal Life (Sec)", DefaultValue = 60)]
        public int MinSignalLifeSeconds { get; set; }

        // --- Contesto Direzionale ---
        [Parameter("Stoch Zone Weight", DefaultValue = 0.10)]
        public double StochZoneWeight { get; set; }

        [Parameter("Stoch Cross Weight", DefaultValue = 0.08)]
        public double StochCrossWeight { get; set; }

        [Parameter("Elder Trend Strong Weight", DefaultValue = 0.10)]
        public double ElderTrendStrongWeight { get; set; }

        [Parameter("Elder Trend Weak Weight", DefaultValue = 0.05)]
        public double ElderTrendWeakWeight { get; set; }

        [Parameter("Squeeze Fire Weight", DefaultValue = 0.12)]
        public double SqueezeFireWeight { get; set; }

        [Parameter("Squeeze Momentum Weight", DefaultValue = 0.05)]
        public double SqueezeMomentumWeight { get; set; }

        [Parameter("EOM Level Weight", DefaultValue = 0.08)]
        public double EOMLevelWeight { get; set; }

        [Parameter("EOM Reversal Weight", DefaultValue = 0.05)]
        public double EOMReversalWeight { get; set; }

        [Parameter("EOM Threshold", DefaultValue = 0.5)]
        public double EOMThreshold { get; set; }

        [Parameter("Hourly Bias Weight", DefaultValue = 0.10)]
        public double HourlyBiasWeight { get; set; }

        [Parameter("Aftermath Bias Weight", DefaultValue = 1.5)]
        public double AftermathBiasWeight { get; set; }

        [Parameter("RSquared Amplifier", DefaultValue = 1.3)]
        public double RSquaredAmplifier { get; set; }

        [Parameter("RSquared Dampener", DefaultValue = 0.7)]
        public double RSquaredDampener { get; set; }

        [Parameter("Aligned Boost Factor", DefaultValue = 0.10)]
        public double AlignedBoostFactor { get; set; }

        [Parameter("Opposed Penalty Factor", DefaultValue = 0.08)]
        public double OpposedPenaltyFactor { get; set; }

        // --- Bias Orario ---
        [Parameter("Bias UTC Offset", DefaultValue = 1)]
        public int BiasUTCOffset { get; set; }

        [Parameter("Bias Lookback Days", DefaultValue = 30, MinValue = 1)]
        public int BiasLookbackDays { get; set; }

        // --- SL/TP ---
        [Parameter("SL Percentile", DefaultValue = 75, MinValue = 1, MaxValue = 99)]
        public int SLPercentile { get; set; }

        [Parameter("Worst Case Percentile", DefaultValue = 95, MinValue = 1, MaxValue = 99)]
        public int WorstCasePercentile { get; set; }

        // --- Display ---
        [Parameter("Show Panel", DefaultValue = true)]
        public bool ShowPanel { get; set; }

        [Parameter("Panel Position", DefaultValue = "TopLeft")]
        public string PanelPosition { get; set; }

        [Parameter("Panel UTC Offset", DefaultValue = 1)]
        public int PanelUTCOffset { get; set; }

        [Parameter("Ghost Bull Color", DefaultValue = "120,0,200,0")]
        public string GhostBullColorStr { get; set; }

        [Parameter("Ghost Bear Color", DefaultValue = "120,200,0,0")]
        public string GhostBearColorStr { get; set; }

        [Parameter("Ghost Neutral Color", DefaultValue = "80,128,128,128")]
        public string GhostNeutralColorStr { get; set; }

        // --- Pattern Library ---
        [Parameter("Pattern Confirm Depth", DefaultValue = 15000, MinValue = 100)]
        public int PatternConfirmDepth { get; set; }

        // --- HTF FILTER (Heikin Ashi) ---
        [Parameter("Use HTF Filter", DefaultValue = true, Group = "Filters")]
        public bool UseHtfFilter { get; set; }

        [Parameter("HTF Timeframe", DefaultValue = "Hour", Group = "Filters")]
        public string HtfTimeframeStr { get; set; }

        // --- TRADING (Bot) ---
        [Parameter("Enable Trading", DefaultValue = true, Group = "Trading")]
        public bool EnableTrading { get; set; }

        [Parameter("Max Spread (Pips)", DefaultValue = 2.0, Group = "Trading")]
        public double MaxSpreadPips { get; set; }

        [Parameter("Trading Start Hour", DefaultValue = 8, Group = "Trading")]
        public int TradingStartHour { get; set; }

        [Parameter("Trading End Hour", DefaultValue = 22, Group = "Trading")]
        public int TradingEndHour { get; set; }

        [Parameter("Trade Volume (Lots)", DefaultValue = 0.01, MinValue = 0.01, Step = 0.01, Group = "Trading")]
        public double TradeVolumeLots { get; set; }

        [Parameter("Trade Label", DefaultValue = "VIT_Bot", Group = "Trading")]
        public string TradeLabel { get; set; }

        // ============================================================
        // 2. COSTANTI INTERNE (non configurabili)
        // ============================================================

        private const int ATR_PERIOD = 14;
        private const int VOLUME_SMA_PERIOD = 20;
        private const double CONFIDENCE_WEIGHT_IS = 0.60;
        private const double CONFIDENCE_WEIGHT_RT = 0.40;
        private const double RT_START_VALUE = 50.0;
        private const double RT_DELTA_MULTIPLIER_TICK = 20.0;
        private const double RT_DELTA_MULTIPLIER_CLOSE = 30.0;

        // Squeeze Momentum parametri
        private const int SQUEEZE_BB_PERIOD = 20;
        private const double SQUEEZE_BB_MULT = 2.0;
        private const int SQUEEZE_KC_PERIOD = 20;
        private const double SQUEEZE_KC_MULT = 1.5;
        private const int SQUEEZE_LRF_PERIOD = 20;
        private const int SQUEEZE_MOM_PERIOD = 20;

        // Pesi similarita (7 metriche, somma = 1.0)
        private const double W_DIRECTION = 0.15;
        private const double W_BODY_RATIO = 0.22;
        private const double W_CLOSE_POS = 0.22;
        private const double W_UPPER_WICK = 0.10;
        private const double W_LOWER_WICK = 0.10;
        private const double W_VOLUME_REL = 0.10;
        private const double W_RANGE_REL = 0.11;

        // ============================================================
        // 3. STRUTTURE DATI & OBJECT POOLING
        // ============================================================

        public class ObjectPool<T> where T : class, new()
        {
            private readonly Stack<T> _pool = new Stack<T>();
            private readonly Action<T> _resetAction;

            public ObjectPool(int initialSize, Action<T> resetAction)
            {
                _resetAction = resetAction;
                for (int i = 0; i < initialSize; i++)
                    _pool.Push(new T());
            }

            public T Get()
            {
                if (_pool.Count > 0)
                {
                    var item = _pool.Pop();
                    _resetAction?.Invoke(item);
                    return item;
                }
                var newItem = new T();
                _resetAction?.Invoke(newItem);
                return newItem;
            }

            public void Return(T item)
            {
                if (item == null) return;
                _pool.Push(item);
            }
        }

        public class CandleFingerprint
        {
            public double Direction;
            public double BodyRatio;
            public double ClosePosition;
            public double UpperWickRatio;
            public double LowerWickRatio;
            public double VolumeRelative;
            public double RangeRelative;
        }

        public class GhostCandle
        {
            public double Open;
            public double High;
            public double Low;
            public double Close;
            public DateTime ProjectedTime;
            public bool IsBullish;

            public void Reset()
            {
                Open = High = Low = Close = 0;
                ProjectedTime = default;
                IsBullish = false;
            }
        }

        public enum CandidateSource
        {
            Historical,
            PatternLibrary
        }

        public enum AftermathOutcome
        {
            Long,
            Short,
            Range
        }

        public class Candidate
        {
            public int HistoryPosition;
            public double InitialSimilarity;
            public double RealtimeScore;
            public double ConfidenceScore;
            public double SuccessRate;
            public CandidateSource Source;
            public string PatternName;
            public bool IsActive;
            public bool IsBuy;
            public AftermathOutcome PredictedOutcome;
            public List<GhostCandle> GhostCandles = new List<GhostCandle>();
            public DateTime EntryTime;
            public int HourSlot;
            public bool WasFoundInLastScan;
            public int CandlesElapsed;

            public void Reset()
            {
                HistoryPosition = 0;
                InitialSimilarity = 0;
                RealtimeScore = 0;
                ConfidenceScore = 0;
                SuccessRate = 0;
                Source = CandidateSource.Historical;
                PatternName = null;
                IsActive = false;
                IsBuy = false;
                PredictedOutcome = AftermathOutcome.Range;
                GhostCandles.Clear();
                EntryTime = default;
                HourSlot = 0;
                WasFoundInLastScan = false;
                CandlesElapsed = 0;
            }
        }

        public class FixedSignalSet
        {
            public bool IsBuy;
            public AftermathOutcome Outcome;
            public List<GhostCandle> SignalGhosts = new List<GhostCandle>();
            public double FinalConfidence;
            public int GhostsRemaining;
            public double StopLossLevel;
            public double TakeProfitLevel;
            public double RiskReward;
            public double WorstCaseLevel;
            public double RangeHigh;
            public double RangeLow;
            public bool IsValid;
            public string SourceInfo;
        }

        public class AftermathData
        {
            public double WeightedCountLong;
            public double WeightedCountShort;
            public double WeightedCountRange;
            public int TotalOccurrences;
            public List<double> DrawdownsLong = new List<double>();
            public List<double> ProfitsLong = new List<double>();
            public List<double> DrawdownsShort = new List<double>();
            public List<double> ProfitsShort = new List<double>();
            public List<double> RangeHighs = new List<double>();
            public List<double> RangeLows = new List<double>();
        }

        public class AftermathEntry
        {
            public int StartBar;
            public int Count;
            public double HistoricalATR;
            public double Similarity;
            public AftermathOutcome Outcome;
        }

        public class DirectionalContext
        {
            public double StochContrib;
            public double ElderContrib;
            public double SqueezeContrib;
            public double EOMContrib;
            public double HourlyBiasContrib;
            public double AftermathBiasContrib;
            public double RSquaredMultiplier;
            public double FinalContext;
        }

        public class NativeIndicatorContext
        {
            public double StochK;
            public double StochD;
            public double StochK_Prev;
            public double StochD_Prev;
            public double ElderBullPower;
            public double ElderBearPower;
            public double[] ElderBullHistory = new double[5];
            public double[] ElderBearHistory = new double[5];
            public double SqueezeMomentum;
            public double SqueezeMomentum_Prev;
            public bool SqueezeIsOn;
            public bool SqueezeIsOn_Prev;
            public bool SqueezeFired;
            public double EaseOfMovement;
            public double EOM_Prev;
            public double RSquared;
        }

        public class PredictorState
        {
            public List<Candidate> ActiveCandidates = new List<Candidate>();
            public Candidate CurrentFavorite;
            public FixedSignalSet ActiveSignal;
            public DateTime SignalCreationTime; // Per Time-Lock
            public DateTime LastSignalDirectionChange; // Aggiunto per Time-Lock inversione
            public bool IsInResetState;
            public DateTime LastFullScanTime;
            public int TotalPositionsScanned;
            public AftermathData GlobalAftermath = new AftermathData();
            public DirectionalContext Context = new DirectionalContext();
            public NativeIndicatorContext Indicators = new NativeIndicatorContext();
            
            // Per Smoothing (SMA 3)
            public List<double> PctLongHistory = new List<double>();
            public List<double> PctShortHistory = new List<double>();
            public List<double> ContextHistory = new List<double>();
            
            // Supporto per il bonus continuità
            public HashSet<int> LastScanPositions = new HashSet<int>();

            // Caching Fingerprint (Performance)
            public CandleFingerprint[] FingerprintCache;
            public int LastCachedBar = -1;
            
            public double PctLong;
            public double PctShort;
            public double PctRange;
            public double CalculatedSLLong;
            public double CalculatedTPLong;
            public double CalculatedSLShort;
            public double CalculatedTPShort;
            public double CalculatedRangeHigh;
            public double CalculatedRangeLow;
            public double WorstCaseLong;
            public double WorstCaseShort;
        }

        // ============================================================
        // 4. INDICATORI NATIVI — Campi e Riferimenti
        // ============================================================

        // Indicatori nativi cTrader
        private StochasticOscillator _stoch;
        private ElderRayIndex _elderRay;
        private BollingerBands _bbIndicator;
        private MovingAverage _kcMA;
        private AverageTrueRange _atrForKC;
        private EaseOfMovement _eom;
        private LinearRegressionRSquared _rsq;
        private AverageTrueRange _atr;
        private MovingAverage _volumeSMA;

        // DataSeries custom per Squeeze Momentum (LRF input)
        private IndicatorDataSeries _squeezeMomSeries;

        // Stato principale
        private PredictorState State;
        private List<Candidate> ActiveCandidates;
        private AftermathData GlobalAftermath;
        private NativeIndicatorContext IndicatorsCtx;

        // Variabili di lavoro
        private string instanceId;
        private int _lastIndex = -1;
        private bool _isProcessing;
        private bool _firstLiveBarDone;
        private double _barPeriodMinutes;

        // Bot: stato per retry di apertura fallita
        private FixedSignalSet _pendingOpenSignal = null;
        private int _pendingOpenAttempts = 0;

        // HTF (Higher Time Frame)
        private Bars _htfBars;

        // Bias orario: array 24 slot, "BUY" / "SELL" / "NEUTRAL"
        private string[] _hourlyBias = Enumerable.Repeat("NEUTRAL", 24).ToArray();

        // Cache colori ghost (inizializzati in Initialize, evita ParseColor ad ogni render)
        private Color _ghostBullColor;
        private Color _ghostBearColor;
        private Color _ghostNeutralColor;

        // Tracciamento nomi ghost disegnati per evitare di iterare su Chart.Objects
        // ad ogni RenderGhosts (o ClearGhostRendering). Ogni nome aggiunto qui
        // deve corrispondere a un oggetto effettivamente creato sul chart dallo
        // stesso metodo che ha fatto l'Add, e rimosso quando l'oggetto viene
        // cancellato. Vedi RenderGhosts / ClearGhostRendering.
        private readonly HashSet<string> _renderedGhostNames = new HashSet<string>();

        // Pools for memory management
        private ObjectPool<Candidate> _candidatePool;
        private ObjectPool<GhostCandle> _ghostPool;

        // Pattern Library (popolata in Initialize con 78 pattern)
        private List<IPatternDefinition> PatternDefinitions = new List<IPatternDefinition>();

        // ============================================================
        // INTERFACE PATTERN LIBRARY (predisposizione)
        // ============================================================

        public interface IPatternDefinition
        {
            string Name { get; }
            CandidateSource Source { get; }
            int RequiredCandles { get; }
            int GhostCandleCount { get; }
            bool IsBullish { get; }
            bool CheckPattern(Bars bars, int endIndex);
            List<GhostCandle> GenerateGhosts(Bars bars, int endIndex, double currentPrice);
        }

        // ============================================================
        // SWING POINT — struttura per FindSwings
        // ============================================================

        public class SwingPoint
        {
            public double Price;
            public int Index;    // indice assoluto barra
            public bool IsHigh;  // true = swing high, false = swing low
        }

        // ============================================================
        // PATTERN BASE — classe astratta per tutti i pattern
        // ============================================================

        public abstract class PatternBase : IPatternDefinition
        {
            protected readonly VITPredittore_v2_2 _ind;

            protected PatternBase(VITPredittore_v2_2 ind)
            {
                _ind = ind;
            }

            // --- Membri dall'interfaccia ---
            public abstract string Name { get; }
            public virtual CandidateSource Source => CandidateSource.PatternLibrary;
            public abstract int RequiredCandles { get; }
            public abstract int GhostCandleCount { get; }
            public virtual bool IsBullish
            {
                get
                {
                    string n = Name ?? string.Empty;
                    // Convenzione prefisso Bull/Bear copre la maggior parte dei pattern
                    if (n.StartsWith("Bull")) return true;
                    if (n.StartsWith("Bear")) return false;
                    // Pattern senza prefisso: direzione derivata dalla semantica
                    switch (n)
                    {
                        case "Double Bottom":
                        case "Falling Wedge":
                        case "Asc Triangle":
                        case "Rising Three Methods":
                        case "Inverse H&S":
                        case "Diamond Bottom":
                        case "Pipe Bottom":
                            return true;
                        case "Double Top":
                        case "Rising Wedge":
                        case "Desc Triangle":
                        case "Falling Three Methods":
                        case "Mini H&S":
                        case "Diamond Top":
                        case "Pipe Top":
                            return false;
                        default:
                            return true; // fallback: sovrascrivere in nuovi pattern
                    }
                }
            }
            public abstract bool CheckPattern(Bars bars, int endIndex);
            public abstract List<GhostCandle> GenerateGhosts(Bars bars, int endIndex, double currentPrice);

            // --- Accesso indicatore ---
            protected double LocalATR(int barIndex)
            {
                return _ind.CalcolaATR(barIndex);
            }

            protected double BarPeriodMinutes => _ind._barPeriodMinutes;

            // --- Indicizzazione relativa ---
            // I pattern usano c[0]..c[RC-1] dove c[0] = endIndex - RC + 1
            protected int Abs(int rel, int endIndex)
            {
                return endIndex - RequiredCandles + 1 + rel;
            }

            protected double O(Bars bars, int endIndex, int rel)
            {
                return bars.OpenPrices[Abs(rel, endIndex)];
            }

            protected double H(Bars bars, int endIndex, int rel)
            {
                return bars.HighPrices[Abs(rel, endIndex)];
            }

            protected double L(Bars bars, int endIndex, int rel)
            {
                return bars.LowPrices[Abs(rel, endIndex)];
            }

            protected double C(Bars bars, int endIndex, int rel)
            {
                return bars.ClosePrices[Abs(rel, endIndex)];
            }

            protected double V(Bars bars, int endIndex, int rel)
            {
                return bars.TickVolumes[Abs(rel, endIndex)];
            }

            protected double Range(Bars bars, int endIndex, int rel)
            {
                int idx = Abs(rel, endIndex);
                return bars.HighPrices[idx] - bars.LowPrices[idx];
            }

            protected double Body(Bars bars, int endIndex, int rel)
            {
                int idx = Abs(rel, endIndex);
                return Math.Abs(bars.ClosePrices[idx] - bars.OpenPrices[idx]);
            }

            protected bool IsBull(Bars bars, int endIndex, int rel)
            {
                int idx = Abs(rel, endIndex);
                return bars.ClosePrices[idx] > bars.OpenPrices[idx];
            }

            protected bool IsBear(Bars bars, int endIndex, int rel)
            {
                int idx = Abs(rel, endIndex);
                return bars.ClosePrices[idx] < bars.OpenPrices[idx];
            }

            protected double Mid(Bars bars, int endIndex, int rel)
            {
                int idx = Abs(rel, endIndex);
                return (bars.HighPrices[idx] + bars.LowPrices[idx]) / 2.0;
            }

            // --- FibCheck ---
            protected bool FibCheck(double value, double target, double tolerance)
            {
                return Math.Abs(value - target) <= tolerance;
            }

            // --- FindSwings ---
            // Trova swing highs e swing lows alternati in un range di barre.
            // startIndex e endIndex sono indici assoluti.
            // Ritorna lista cronologica con alternanza forzata.
            protected List<SwingPoint> FindSwings(Bars bars, int startIndex, int endIndex)
            {
                var rawSwings = new List<SwingPoint>();

                // Clamp ai limiti delle barre disponibili
                if (startIndex < 1) startIndex = 1;
                if (endIndex >= bars.Count - 1) endIndex = bars.Count - 2;
                if (startIndex >= endIndex) return rawSwings;

                // Fase 1: identifica tutti gli swing grezzi
                // NOTA: Non-stretto (>=, <=): in caso di parita' (prezzi flat),
                // la barra piu' a sinistra della regione diventa lo swing
                // dopo l'alternanza (viene preservata come "prima estrema").
                for (int i = startIndex; i <= endIndex; i++)
                {
                    bool isHigh = bars.HighPrices[i] >= bars.HighPrices[i - 1]
                               && bars.HighPrices[i] >= bars.HighPrices[i + 1];
                    bool isLow = bars.LowPrices[i] <= bars.LowPrices[i - 1]
                              && bars.LowPrices[i] <= bars.LowPrices[i + 1];

                    if (isHigh && isLow)
                    {
                        // Candela e' sia high che low (doji/spike):
                        // se il range e' significativo, aggiungi entrambi.
                        // Ordine arbitrario: low poi high (entrambi dalla stessa
                        // barra, non esiste ordine cronologico intra-bar). Nota:
                        // l'ordine influenza l'alternanza quando il previous swing
                        // e' dello stesso tipo.
                        // NOTA: due swing point vengono aggiunti allo stesso Index.
                        // I pattern che usano differenze di Index devono avere
                        // guard denom <= 0 prima di dividere.
                        double range = bars.HighPrices[i] - bars.LowPrices[i];
                        if (range > 0.0001)
                        {
                            rawSwings.Add(new SwingPoint
                            {
                                Price = bars.LowPrices[i],
                                Index = i,
                                IsHigh = false
                            });
                            rawSwings.Add(new SwingPoint
                            {
                                Price = bars.HighPrices[i],
                                Index = i,
                                IsHigh = true
                            });
                        }
                    }
                    else if (isHigh)
                    {
                        rawSwings.Add(new SwingPoint
                        {
                            Price = bars.HighPrices[i],
                            Index = i,
                            IsHigh = true
                        });
                    }
                    else if (isLow)
                    {
                        rawSwings.Add(new SwingPoint
                        {
                            Price = bars.LowPrices[i],
                            Index = i,
                            IsHigh = false
                        });
                    }
                }

                if (rawSwings.Count < 2) return rawSwings;

                // Fase 2: forza alternanza high/low
                // Se due consecutivi sono dello stesso tipo,
                // tieni quello con il prezzo piu' estremo
                var alternated = new List<SwingPoint>();
                alternated.Add(rawSwings[0]);

                for (int i = 1; i < rawSwings.Count; i++)
                {
                    var last = alternated[alternated.Count - 1];
                    var current = rawSwings[i];

                    if (current.IsHigh == last.IsHigh)
                    {
                        // Stesso tipo: tieni il piu' estremo
                        if (current.IsHigh)
                        {
                            // Due highs: tieni il piu' alto
                            if (current.Price > last.Price)
                                alternated[alternated.Count - 1] = current;
                        }
                        else
                        {
                            // Due lows: tieni il piu' basso
                            if (current.Price < last.Price)
                                alternated[alternated.Count - 1] = current;
                        }
                    }
                    else
                    {
                        // Tipo diverso: aggiungi normalmente
                        alternated.Add(current);
                    }
                }

                return alternated;
            }

            // --- GenerateGhostsToTarget ---
            // Genera ghost candle con accelerazione e wick asimmetriche.
            // Ogni pattern chiama questa funzione passando solo il target.
            protected List<GhostCandle> GenerateGhostsToTarget(
                Bars bars, int endIndex, double currentPrice,
                double targetPrice, int ghostCount)
            {
                var ghosts = new List<GhostCandle>();

                // Guard: ghostCount non positivo -> lista vuota
                // (protegge sia da new double[ghostCount] con valore negativo,
                //  sia dalla divisione Math.Abs(totalMove)/ghostCount con zero)
                if (ghostCount <= 0) return ghosts;

                double totalMove = targetPrice - currentPrice;

                // Caso limite: nessun movimento → lista vuota
                // (il candidato verra' scartato da ProcessPatternCandidate)
                if (Math.Abs(totalMove) < 0.0001)
                    return ghosts;

                double localATR = LocalATR(endIndex);
                bool isBullish = totalMove > 0;

                // Range medio per ghost (usato per le wick)
                double avgRange = localATR > 0
                    ? localATR * 0.8
                    : Math.Abs(totalMove) / ghostCount;

                // Pesi di accelerazione (breakout: lento all'inizio, veloce alla fine)
                // La somma dei pesi e' sempre 1.0
                double[] weights;
                if (ghostCount == 4)
                    weights = new double[] { 0.15, 0.20, 0.25, 0.40 };
                else if (ghostCount == 5)
                    weights = new double[] { 0.10, 0.15, 0.20, 0.25, 0.30 };
                else
                {
                    // Fallback per qualsiasi ghostCount: distribuzione lineare crescente
                    weights = new double[ghostCount];
                    double sumWeights = 0;
                    for (int w = 0; w < ghostCount; w++)
                    {
                        weights[w] = w + 1; // 1, 2, 3, ...
                        sumWeights += weights[w];
                    }
                    for (int w = 0; w < ghostCount; w++)
                        weights[w] /= sumWeights; // normalizza a somma 1.0
                }

                // baseTime: stessa logica di BuildAndScaleGhosts (Sezione 14)
                DateTime baseTime;
                if (endIndex + 1 < bars.Count)
                    baseTime = bars.OpenTimes[endIndex + 1];
                else
                    baseTime = bars.OpenTimes[endIndex]
                               + TimeSpan.FromMinutes(BarPeriodMinutes);

                double cumulativeWeight = 0;

                for (int i = 0; i < ghostCount; i++)
                {
                    cumulativeWeight += weights[i];

                    double ghostOpen;
                    if (i == 0)
                        ghostOpen = currentPrice;
                    else
                        ghostOpen = ghosts[i - 1].Close;

                    double ghostClose = currentPrice + totalMove * cumulativeWeight;

                    // Body
                    double bodyTop = Math.Max(ghostOpen, ghostClose);
                    double bodyBottom = Math.Min(ghostOpen, ghostClose);
                    double body = bodyTop - bodyBottom;

                    // Wick proporzionali con asimmetria direzionale
                    double wickAllowance = Math.Max(avgRange - body, 0) * 0.5;

                    double ghostHigh, ghostLow;
                    if (isBullish)
                    {
                        // Bull: wick inferiore piu' lunga (supporto testato)
                        ghostHigh = bodyTop + wickAllowance * 0.35;
                        ghostLow = bodyBottom - wickAllowance * 0.65;
                    }
                    else
                    {
                        // Bear: wick superiore piu' lunga (resistenza testata)
                        ghostHigh = bodyTop + wickAllowance * 0.65;
                        ghostLow = bodyBottom - wickAllowance * 0.35;
                    }

                    // Clamp di sicurezza: invariante OHLC
                    // BuildFingerprintFromGhost calcola UpperWickRatio e LowerWickRatio
                    // che diventano negativi se questo invariante e' violato
                    ghostHigh = Math.Max(ghostHigh, Math.Max(ghostOpen, ghostClose));
                    ghostLow = Math.Min(ghostLow, Math.Min(ghostOpen, ghostClose));
                    if (ghostHigh <= ghostLow)
                        ghostHigh = ghostLow + 0.0001;

                    ghosts.Add(new GhostCandle
                    {
                        Open = ghostOpen,
                        High = ghostHigh,
                        Low = ghostLow,
                        Close = ghostClose,
                        ProjectedTime = baseTime
                                        + TimeSpan.FromMinutes(BarPeriodMinutes * i),
                        IsBullish = ghostClose > ghostOpen
                    });
                }

                return ghosts;
            }
        }

        // ============================================================
        // 4.6 UpdateNativeIndicators — PARTE 1: Read-only (ogni tick)
        // ============================================================

        private void UpdateNativeIndicators(int index)
        {
            // Stochastic — accesso indicizzato per coerenza con il parametro index
            IndicatorsCtx.StochK = _stoch.PercentK[index];
            IndicatorsCtx.StochD = _stoch.PercentD[index];
            if (index > 0)
            {
                IndicatorsCtx.StochK_Prev = _stoch.PercentK[index - 1];
                IndicatorsCtx.StochD_Prev = _stoch.PercentD[index - 1];
            }

            // Elder Ray (valori correnti, NO shift qui)
            IndicatorsCtx.ElderBullPower = _elderRay.BullsPower[index];
            IndicatorsCtx.ElderBearPower = _elderRay.BearsPower[index];

            // Squeeze Momentum — calcolo
            double bbUpper = _bbIndicator.Top[index];
            double bbLower = _bbIndicator.Bottom[index];
            double kcUpper = _kcMA.Result[index] +
                             (_atrForKC.Result[index] * SQUEEZE_KC_MULT);
            double kcLower = _kcMA.Result[index] -
                             (_atrForKC.Result[index] * SQUEEZE_KC_MULT);

            IndicatorsCtx.SqueezeIsOn = bbUpper < kcUpper && bbLower > kcLower;

            // Momentum: Close - midline(highest+lowest over 20)
            // Calcolo manuale della regressione lineare (LRF)
            if (index >= SQUEEZE_MOM_PERIOD)
            {
                // Aggiorna la serie momentum per la barra corrente
                double highest = double.MinValue;
                double lowest = double.MaxValue;
                for (int i = index - SQUEEZE_MOM_PERIOD + 1; i <= index; i++)
                {
                    if (Bars.HighPrices[i] > highest) highest = Bars.HighPrices[i];
                    if (Bars.LowPrices[i] < lowest) lowest = Bars.LowPrices[i];
                }
                double midline = (highest + lowest) / 2.0;
                _squeezeMomSeries[index] = Bars.ClosePrices[index] - midline;

                // Regressione lineare solo quando ci sono abbastanza barre popolate
                if (index >= SQUEEZE_MOM_PERIOD + SQUEEZE_LRF_PERIOD - 1)
                {
                    IndicatorsCtx.SqueezeMomentum = CalculateLinearRegressionForecast(
                        _squeezeMomSeries, index, SQUEEZE_LRF_PERIOD);
                }
            }

            // SqueezeFired: usa SqueezeIsOn_Prev salvato a bar close
            IndicatorsCtx.SqueezeFired = IndicatorsCtx.SqueezeIsOn_Prev
                                         && !IndicatorsCtx.SqueezeIsOn;

            // Ease of Movement
            IndicatorsCtx.EaseOfMovement = _eom.Result[index];
            if (index > 0)
                IndicatorsCtx.EOM_Prev = _eom.Result[index - 1];

            // R-Squared
            IndicatorsCtx.RSquared = _rsq.Result[index];
        }

        // ============================================================
        // 4.6 ShiftIndicatorHistory — PARTE 2: SOLO a chiusura candela
        // ============================================================

        private void ShiftIndicatorHistory(int closedBarIndex)
        {
            // Shift Elder History
            for (int i = 0; i < 4; i++)
            {
                IndicatorsCtx.ElderBullHistory[i] = IndicatorsCtx.ElderBullHistory[i + 1];
                IndicatorsCtx.ElderBearHistory[i] = IndicatorsCtx.ElderBearHistory[i + 1];
            }
            // Usa l'indice della barra chiusa per ottenere i valori corretti,
            // perche IndicatorsCtx e gia stato sovrascritto da UpdateNativeIndicators
            // con i valori della nuova barra prima che OnBarClosed venga chiamato.
            IndicatorsCtx.ElderBullHistory[4] = _elderRay.BullsPower[closedBarIndex];
            IndicatorsCtx.ElderBearHistory[4] = _elderRay.BearsPower[closedBarIndex];

            // Squeeze: ricalcola lo stato per la barra chiusa (non usare IndicatorsCtx
            // che contiene gia i valori della nuova barra)
            double bbUpper = _bbIndicator.Top[closedBarIndex];
            double bbLower = _bbIndicator.Bottom[closedBarIndex];
            double kcUpper = _kcMA.Result[closedBarIndex]
                             + (_atrForKC.Result[closedBarIndex] * SQUEEZE_KC_MULT);
            double kcLower = _kcMA.Result[closedBarIndex]
                             - (_atrForKC.Result[closedBarIndex] * SQUEEZE_KC_MULT);
            IndicatorsCtx.SqueezeIsOn_Prev = bbUpper < kcUpper && bbLower > kcLower;

            // SqueezeMomentum_Prev: ricalcola per la barra chiusa
            if (closedBarIndex >= SQUEEZE_MOM_PERIOD + SQUEEZE_LRF_PERIOD - 1)
            {
                IndicatorsCtx.SqueezeMomentum_Prev = CalculateLinearRegressionForecast(
                    _squeezeMomSeries, closedBarIndex, SQUEEZE_LRF_PERIOD);
            }
        }

        // ============================================================
        // Helper: Minuti dal TimeFrame corrente
        // ============================================================

        private double GetTimeFrameMinutes()
        {
            var tf = TimeFrame;
            if (tf == TimeFrame.Minute)   return 1;
            if (tf == TimeFrame.Minute2)  return 2;
            if (tf == TimeFrame.Minute3)  return 3;
            if (tf == TimeFrame.Minute4)  return 4;
            if (tf == TimeFrame.Minute5)  return 5;
            if (tf == TimeFrame.Minute6)  return 6;
            if (tf == TimeFrame.Minute7)  return 7;
            if (tf == TimeFrame.Minute8)  return 8;
            if (tf == TimeFrame.Minute9)  return 9;
            if (tf == TimeFrame.Minute10) return 10;
            if (tf == TimeFrame.Minute15) return 15;
            if (tf == TimeFrame.Minute20) return 20;
            if (tf == TimeFrame.Minute30) return 30;
            if (tf == TimeFrame.Minute45) return 45;
            if (tf == TimeFrame.Hour)     return 60;
            if (tf == TimeFrame.Hour2)    return 120;
            if (tf == TimeFrame.Hour3)    return 180;
            if (tf == TimeFrame.Hour4)    return 240;
            if (tf == TimeFrame.Hour6)    return 360;
            if (tf == TimeFrame.Hour8)    return 480;
            if (tf == TimeFrame.Hour12)   return 720;
            if (tf == TimeFrame.Daily)    return 1440;
            if (tf == TimeFrame.Day2)     return 2880;
            if (tf == TimeFrame.Day3)     return 4320;
            if (tf == TimeFrame.Weekly)   return 10080;
            if (tf == TimeFrame.Monthly)  return 43200;
            return 5; // fallback M5
        }

        // ============================================================
        // Helper: Regressione Lineare Forecast (manuale)
        // ============================================================

        private double CalculateLinearRegressionForecast(
            IndicatorDataSeries series, int endIndex, int period)
        {
            int startIndex = endIndex - period + 1;
            if (startIndex < 0) startIndex = 0;
            int n = endIndex - startIndex + 1;
            if (n < 2) return series[endIndex];

            double sumX = 0, sumY = 0, sumXY = 0, sumX2 = 0;
            for (int i = 0; i < n; i++)
            {
                double x = i;
                double y = series[startIndex + i];
                sumX += x;
                sumY += y;
                sumXY += x * y;
                sumX2 += x * x;
            }

            double denominator = n * sumX2 - sumX * sumX;
            if (Math.Abs(denominator) < 1e-10)
                return series[endIndex];

            double slope = (n * sumXY - sumX * sumY) / denominator;
            double intercept = (sumY - slope * sumX) / n;

            // Forecast: valore proiettato alla posizione n-1 (ultimo punto)
            return intercept + slope * (n - 1);
        }

        // ============================================================
        // Helper: CalcolaATR manuale per posizioni storiche
        // ============================================================

        private double CalcolaATR(int barIndex)
        {
            // Usa l'indicatore ATR nativo pre-inizializzato (ATR_PERIOD = 14)
            if (barIndex >= 0 && barIndex < Bars.Count)
                return _atr.Result[barIndex];
            return 0;
        }

        // ============================================================
        // Helper: SMA Volume
        // ============================================================

        private double SMA_Volume(int barIndex)
        {
            // Usa l'indicatore SMA Volume pre-inizializzato (VOLUME_SMA_PERIOD = 20)
            if (barIndex >= 0 && barIndex < Bars.Count)
            {
                double val = _volumeSMA.Result[barIndex];
                return val > 0 ? val : 1.0;
            }
            return 1.0;
        }

        // ============================================================
        // Helper: ParseColor da stringa ARGB
        // ============================================================

        private Color ParseColor(string argbStr)
        {
            var parts = argbStr.Split(',');
            if (parts.Length == 4)
            {
                if (int.TryParse(parts[0].Trim(), out int a) &&
                    int.TryParse(parts[1].Trim(), out int r) &&
                    int.TryParse(parts[2].Trim(), out int g) &&
                    int.TryParse(parts[3].Trim(), out int b))
                {
                    a = Math.Max(0, Math.Min(255, a));
                    r = Math.Max(0, Math.Min(255, r));
                    g = Math.Max(0, Math.Min(255, g));
                    b = Math.Max(0, Math.Min(255, b));
                    return Color.FromArgb(a, r, g, b);
                }
            }
            return Color.White;
        }

        // ============================================================
        // Helper: GetCurrentHourSlot / GetHourSlot
        // ============================================================

        private int GetCurrentHourSlot()
        {
            return ((Server.TimeInUtc.Hour + BiasUTCOffset) % 24 + 24) % 24;
        }

        private int GetHourSlot(DateTime time)
        {
            return ((time.Hour + BiasUTCOffset) % 24 + 24) % 24;
        }

        // ============================================================
        // Helper: GetVerticalAlignment / GetHorizontalAlignment
        // ============================================================

        private VerticalAlignment GetVerticalAlignment()
        {
            if (PanelPosition.Contains("Top")) return VerticalAlignment.Top;
            if (PanelPosition.Contains("Bottom")) return VerticalAlignment.Bottom;
            return VerticalAlignment.Top;
        }

        private HorizontalAlignment GetHorizontalAlignment()
        {
            if (PanelPosition.Contains("Left")) return HorizontalAlignment.Left;
            if (PanelPosition.Contains("Right")) return HorizontalAlignment.Right;
            return HorizontalAlignment.Left;
        }

        // ============================================================
        // Helper: DeepCopyGhost
        // ============================================================

        private GhostCandle DeepCopyGhost(GhostCandle g)
        {
            var copy = _ghostPool.Get();
            copy.Open = g.Open;
            copy.High = g.High;
            copy.Low = g.Low;
            copy.Close = g.Close;
            copy.ProjectedTime = g.ProjectedTime;
            copy.IsBullish = g.IsBullish;
            return copy;
        }

        // ============================================================
        // Helper: GetBiasLabel
        // ============================================================

        private string GetBiasLabel(int hourSlot)
        {
            if (hourSlot >= 0 && hourSlot < 24)
                return _hourlyBias[hourSlot];
            return "NEUTRAL";
        }

        // ============================================================
        // Helper: HTF / Heikin Ashi
        // ============================================================

        private TimeFrame ParseTimeFrame(string tfStr)
        {
            switch (tfStr.ToLower())
            {
                case "minute": return TimeFrame.Minute;
                case "minute2": return TimeFrame.Minute2;
                case "minute3": return TimeFrame.Minute3;
                case "minute4": return TimeFrame.Minute4;
                case "minute5": return TimeFrame.Minute5;
                case "minute10": return TimeFrame.Minute10;
                case "minute15": return TimeFrame.Minute15;
                case "minute30": return TimeFrame.Minute30;
                case "hour": return TimeFrame.Hour;
                case "hour2": return TimeFrame.Hour2;
                case "hour4": return TimeFrame.Hour4;
                case "daily": return TimeFrame.Daily;
                default: return TimeFrame.Hour;
            }
        }

        private bool IsHtfTrendAligned(bool isBuy)
        {
            if (!UseHtfFilter || _htfBars == null || _htfBars.Count < 2)
                return true;

            // Calcolo Heikin Ashi dell'ultima barra HTF chiusa
            int idx = _htfBars.Count - 2;
            
            double prevOpen = _htfBars.OpenPrices[idx - 1];
            double prevClose = _htfBars.ClosePrices[idx - 1];
            double haOpen = (prevOpen + prevClose) / 2.0;
            
            double curOpen = _htfBars.OpenPrices[idx];
            double curHigh = _htfBars.HighPrices[idx];
            double curLow = _htfBars.LowPrices[idx];
            double curClose = _htfBars.ClosePrices[idx];
            double haClose = (curOpen + curHigh + curLow + curClose) / 4.0;

            bool haBullish = haClose > haOpen;
            return isBuy ? haBullish : !haBullish;
        }

        // ============================================================
        // 5. FINGERPRINT — COSTRUZIONE
        // ============================================================

        private CandleFingerprint BuildFingerprint(int barIndex)
        {
            // Lazy initialization or resize
            if (State.FingerprintCache == null || barIndex >= State.FingerprintCache.Length)
            {
                int newSize = Math.Max(Bars.Count + 1000, barIndex + 500);
                var newCache = new CandleFingerprint[newSize];
                if (State.FingerprintCache != null)
                    Array.Copy(State.FingerprintCache, newCache, State.FingerprintCache.Length);
                State.FingerprintCache = newCache;
            }

            // Return cached if valid and not the current live bar (forming bar)
            if (barIndex < Bars.Count - 1 && State.FingerprintCache[barIndex] != null)
                return State.FingerprintCache[barIndex];

            double open = Bars.OpenPrices[barIndex];
            double high = Bars.HighPrices[barIndex];
            double low = Bars.LowPrices[barIndex];
            double close = Bars.ClosePrices[barIndex];
            double volume = Bars.TickVolumes[barIndex];
            double range = high - low;

            if (range < 0.0001)
                return new CandleFingerprint(); // tutti a 0, Direction = 0

            double body = Math.Abs(close - open);
            double avgVol = SMA_Volume(barIndex);
            double atr = CalcolaATR(barIndex - 1);

            var fp = new CandleFingerprint
            {
                Direction = (close > open) ? 1.0 :
                            (close < open) ? -1.0 : 0.0,
                BodyRatio = body / range,
                ClosePosition = (close - low) / range,
                UpperWickRatio = (high - Math.Max(open, close)) / range,
                LowerWickRatio = (Math.Min(open, close) - low) / range,
                VolumeRelative = (avgVol > 0) ? volume / avgVol : 1.0,
                RangeRelative = (atr > 0) ? range / atr : 1.0
            };

            // Cache it if it's a closed bar
            if (barIndex < Bars.Count - 1)
            {
                State.FingerprintCache[barIndex] = fp;
                if (barIndex > State.LastCachedBar) State.LastCachedBar = barIndex;
            }

            return fp;
        }

        private CandleFingerprint[] BuildSignature(int endIndex)
        {
            int startIndex = endIndex - AnalysisWindow + 1;
            if (startIndex < 0) return null;
            var signature = new CandleFingerprint[AnalysisWindow];
            for (int i = 0; i < AnalysisWindow; i++)
            {
                signature[i] = BuildFingerprint(startIndex + i);
            }
            return signature;
        }

        // ============================================================
        // 6. SIMILARITA — CALCOLO PER CANDELA
        // ============================================================

        private double CalculateCandleSimilarity(CandleFingerprint a, CandleFingerprint b)
        {
            double twoSigmaSq = 2.0 * SimilaritySigma * SimilaritySigma;
            if (twoSigmaSq < 1e-15)
                twoSigmaSq = 1e-15;

            // 1. Direction (peso 0.20) — resta lineare, è binaria
            double dirScore;
            if (a.Direction == 0.0 || b.Direction == 0.0)
                dirScore = 1.0 - Math.Abs(a.Direction - b.Direction);
            else
                dirScore = (a.Direction == b.Direction) ? 1.0 : 0.0;

            // 2. BodyRatio (peso 0.18)
            double bodyDiff = a.BodyRatio - b.BodyRatio;
            double bodyScore = Math.Exp(-(bodyDiff * bodyDiff) / twoSigmaSq);

            // 3. ClosePosition (peso 0.18)
            double closeDiff = a.ClosePosition - b.ClosePosition;
            double closeScore = Math.Exp(-(closeDiff * closeDiff) / twoSigmaSq);

            // 4. UpperWickRatio (peso 0.11)
            double upperDiff = a.UpperWickRatio - b.UpperWickRatio;
            double upperScore = Math.Exp(-(upperDiff * upperDiff) / twoSigmaSq);

            // 5. LowerWickRatio (peso 0.11)
            double lowerDiff = a.LowerWickRatio - b.LowerWickRatio;
            double lowerScore = Math.Exp(-(lowerDiff * lowerDiff) / twoSigmaSq);

            // 6. VolumeRelative (peso 0.10)
            // Normalizza la diff dividendo per 2.0 prima della Gaussian
            // così volume doppio della media = diff 0.5, non 1.0
            double volDiff = (a.VolumeRelative - b.VolumeRelative) / 2.0;
            double volScore = Math.Exp(-(volDiff * volDiff) / twoSigmaSq);

            // 7. RangeRelative (peso 0.12)
            double rangeDiff = (a.RangeRelative - b.RangeRelative) / 2.0;
            double rangeScore = Math.Exp(-(rangeDiff * rangeDiff) / twoSigmaSq);

            return dirScore   * W_DIRECTION +
                   bodyScore  * W_BODY_RATIO +
                   closeScore * W_CLOSE_POS +
                   upperScore * W_UPPER_WICK +
                   lowerScore * W_LOWER_WICK +
                   volScore   * W_VOLUME_REL +
                   rangeScore * W_RANGE_REL;
        }

        // ============================================================
        // 7. SIMILARITA — CALCOLO PER SEQUENZA
        // ============================================================

        private double CalculateSequenceSimilarity(
            CandleFingerprint[] signatureA,
            CandleFingerprint[] signatureB)
        {
            if (signatureA.Length != signatureB.Length) return 0.0;
            if (signatureA.Length == 0) return 0.0;

            double total = 0.0;
            for (int i = 0; i < signatureA.Length; i++)
                total += CalculateCandleSimilarity(signatureA[i], signatureB[i]);

            return (total / signatureA.Length) * 100.0; // scala 0-100
        }

        // ============================================================
        // 9. CONTESTO DIREZIONALE — SISTEMA
        // ============================================================

        // 9.1 Calcolo del contesto
        private void UpdateDirectionalContext()
        {
            double sum = 0;

            // --- Stochastic ---
            double stochContrib = 0;
            if (IndicatorsCtx.StochK < 20)
                stochContrib += StochZoneWeight;
            else if (IndicatorsCtx.StochK > 80)
                stochContrib -= StochZoneWeight;

            bool bullCross = IndicatorsCtx.StochK_Prev < IndicatorsCtx.StochD_Prev
                          && IndicatorsCtx.StochK >= IndicatorsCtx.StochD;
            bool bearCross = IndicatorsCtx.StochK_Prev > IndicatorsCtx.StochD_Prev
                          && IndicatorsCtx.StochK <= IndicatorsCtx.StochD;

            if (bullCross && IndicatorsCtx.StochK < 30)
                stochContrib += StochCrossWeight;
            if (bearCross && IndicatorsCtx.StochK > 70)
                stochContrib -= StochCrossWeight;

            sum += stochContrib;

            // --- Elder Ray ---
            double elderContrib = 0;

            // Conta quante volte Bear Power e salita (meno negativa) su 5 candele
            int bearRising = 0;
            for (int i = 1; i < 5; i++)
                if (IndicatorsCtx.ElderBearHistory[i] > IndicatorsCtx.ElderBearHistory[i - 1])
                    bearRising++;
            bool sellerWeakening = bearRising >= 3;

            // Conta quante volte Bull Power e scesa su 5 candele
            int bullFalling = 0;
            for (int i = 1; i < 5; i++)
                if (IndicatorsCtx.ElderBullHistory[i] < IndicatorsCtx.ElderBullHistory[i - 1])
                    bullFalling++;
            bool buyerWeakening = bullFalling >= 3;

            // Inversi
            int bullRising = 0;
            for (int i = 1; i < 5; i++)
                if (IndicatorsCtx.ElderBullHistory[i] > IndicatorsCtx.ElderBullHistory[i - 1])
                    bullRising++;
            bool buyerStrengthening = bullRising >= 3;

            int bearFalling = 0;
            for (int i = 1; i < 5; i++)
                if (IndicatorsCtx.ElderBearHistory[i] < IndicatorsCtx.ElderBearHistory[i - 1])
                    bearFalling++;
            bool sellerStrengthening = bearFalling >= 3;

            if (sellerWeakening) elderContrib += ElderTrendStrongWeight;
            if (buyerWeakening) elderContrib -= ElderTrendStrongWeight;
            if (buyerStrengthening) elderContrib += ElderTrendWeakWeight;
            if (sellerStrengthening) elderContrib -= ElderTrendWeakWeight;

            sum += elderContrib;

            // --- Squeeze Momentum ---
            double squeezeContrib = 0;
            if (IndicatorsCtx.SqueezeFired && IndicatorsCtx.SqueezeMomentum > 0)
                squeezeContrib += SqueezeFireWeight;
            else if (IndicatorsCtx.SqueezeFired && IndicatorsCtx.SqueezeMomentum < 0)
                squeezeContrib -= SqueezeFireWeight;
            else if (!IndicatorsCtx.SqueezeIsOn)
            {
                // Momentum crescente/calante senza squeeze
                if (IndicatorsCtx.SqueezeMomentum > 0 &&
                    IndicatorsCtx.SqueezeMomentum > IndicatorsCtx.SqueezeMomentum_Prev)
                    squeezeContrib += SqueezeMomentumWeight;
                else if (IndicatorsCtx.SqueezeMomentum < 0 &&
                         IndicatorsCtx.SqueezeMomentum < IndicatorsCtx.SqueezeMomentum_Prev)
                    squeezeContrib -= SqueezeMomentumWeight;
            }
            sum += squeezeContrib;

            // --- Ease of Movement ---
            double eomContrib = 0;
            if (IndicatorsCtx.EaseOfMovement > EOMThreshold)
                eomContrib += EOMLevelWeight;
            else if (IndicatorsCtx.EaseOfMovement < -EOMThreshold)
                eomContrib -= EOMLevelWeight;

            if (IndicatorsCtx.EOM_Prev < 0 && IndicatorsCtx.EaseOfMovement > 0)
                eomContrib += EOMReversalWeight;
            else if (IndicatorsCtx.EOM_Prev > 0 && IndicatorsCtx.EaseOfMovement < 0)
                eomContrib -= EOMReversalWeight;

            sum += eomContrib;

            // --- Bias Orario ---
            double biasContrib = 0;
            string bias = GetBiasLabel(GetCurrentHourSlot());
            if (bias == "BUY") biasContrib = HourlyBiasWeight;
            else if (bias == "SELL") biasContrib = -HourlyBiasWeight;
            sum += biasContrib;

            // --- Aftermath Globale ---
            // Il contributo piu forte: dati statistici reali dallo storico
            double aftermathBias = (State.PctLong - State.PctShort) / 100.0;
            double aftermathContrib = aftermathBias * AftermathBiasWeight;
            aftermathContrib = Math.Max(-0.5, Math.Min(0.5, aftermathContrib));
            sum += aftermathContrib;

            // --- R-Squared Moltiplicatore ---
            double rMultiplier = 1.0;
            if (IndicatorsCtx.RSquared > 0.5) rMultiplier = RSquaredAmplifier;
            else if (IndicatorsCtx.RSquared < 0.3) rMultiplier = RSquaredDampener;

            // --- Risultato finale ---
            double rawFinalContext = sum * rMultiplier;
            rawFinalContext = Math.Max(-1.0, Math.Min(1.0, rawFinalContext)); // clamp

            // Smoothing (SMA 3)
            State.ContextHistory.Add(rawFinalContext);
            if (State.ContextHistory.Count > 3) State.ContextHistory.RemoveAt(0);
            double smoothedFinalContext = State.ContextHistory.Average();

            // Salva contributi individuali per il pannello
            State.Context = new DirectionalContext
            {
                StochContrib = stochContrib,
                ElderContrib = elderContrib,
                SqueezeContrib = squeezeContrib,
                EOMContrib = eomContrib,
                HourlyBiasContrib = biasContrib,
                AftermathBiasContrib = aftermathContrib,
                RSquaredMultiplier = rMultiplier,
                FinalContext = smoothedFinalContext
            };
        }

        // 9.2 EffectiveConfidence — Influenza sulla gara
        private double GetEffectiveConfidence(Candidate c)
        {
            double bonus = 0;
            // Bonus continuità: favorisce i candidati già presenti nello scan precedente
            if (c.WasFoundInLastScan) bonus += 2.0;

            // RANGE: il contesto direzionale non li influenza
            if (c.PredictedOutcome == AftermathOutcome.Range)
                return c.ConfidenceScore + bonus;

            double context = State.Context.FinalContext; // -1.0 a +1.0

            // Allineato: contesto positivo E candidato BUY,
            //            oppure contesto negativo E candidato SELL
            bool isAligned = (context > 0 && c.IsBuy) ||
                             (context < 0 && !c.IsBuy);

            double multiplier;
            if (isAligned)
                multiplier = 1.0 + (Math.Abs(context) * AlignedBoostFactor);
            else
                multiplier = 1.0 - (Math.Abs(context) * OpposedPenaltyFactor);

            return Math.Max(0, Math.Min(200, (c.ConfidenceScore + bonus) * multiplier));
        }

        // ============================================================
        // 10. SCAN COMPLETO — FLUSSO PRINCIPALE
        // ============================================================

        private void ExecuteFullScan(int index)
        {
            // 0. Preparazione
            int lastClosed = index - 1;
            var signature = BuildSignature(lastClosed);
            if (signature == null) return;
            
            // === Dinamismo Volatilità ===
            double currentATR = CalcolaATR(lastClosed);
            double avgATR = 0;
            int atrLookback = 50;
            if (lastClosed > atrLookback)
            {
                for (int i = 0; i < atrLookback; i++) avgATR += CalcolaATR(lastClosed - i);
                avgATR /= (double)atrLookback;
            }
            else avgATR = currentATR;
            
            double volRatio = (avgATR > 0) ? currentATR / avgATR : 1.0;
            // Se volRatio > 1 (espansione), ALZIAMO la soglia per essere più selettivi
            // Se volRatio < 1 (compressione), riduciamo leggermente per trovare pattern nel rumore
            double dynamicSimilarityThreshold = SimilarityThreshold * (1.0 + (volRatio - 1.0) * VolatilitySensitivity);
            dynamicSimilarityThreshold = Math.Max(50, Math.Min(98, dynamicSimilarityThreshold));

            // Salva le posizioni dello scan precedente per il bonus continuità
            State.LastScanPositions = new HashSet<int>(ActiveCandidates
                .Where(c => c.Source == CandidateSource.Historical)
                .Select(c => c.HistoryPosition));

            // Reset flag "WasFoundInLastScan" per tutti i candidati attivi
            foreach (var c in State.ActiveCandidates)
                c.WasFoundInLastScan = false;

            GlobalAftermath = new AftermathData();
            State.GlobalAftermath = GlobalAftermath;
            State.TotalPositionsScanned = 0;

            // Lista temporanea per aftermath (TopN)
            var aftermathEntries = new List<AftermathEntry>();

            // 1. Scan finestra scorrevole su HistoryDepth
            int startPos = lastClosed - HistoryDepth;
            int minBar = AnalysisWindow + VOLUME_SMA_PERIOD;
            if (startPos < minBar) startPos = minBar;
            int endPos = lastClosed - AnalysisWindow - AftermathCandles + 1;

            for (int pos = startPos; pos <= endPos; pos++)
            {
                State.TotalPositionsScanned++;
                var histSignature = BuildSignature(pos + AnalysisWindow - 1);
                if (histSignature == null) continue;
                double similarity = CalculateSequenceSimilarity(signature, histSignature);

                // 2. Raccolta aftermath (>= AftermathThreshold)
                //    NON registra subito — accumula in lista temporanea
                if (similarity >= AftermathThreshold)
                {
                    int afterStart = pos + AnalysisWindow;
                    double historicalATR = CalcolaATR(afterStart);
                    AftermathOutcome outcome = ClassifyAftermath(
                        afterStart, AftermathCandles, historicalATR);

                    aftermathEntries.Add(new AftermathEntry
                    {
                        StartBar = afterStart,
                        Count = AftermathCandles,
                        HistoricalATR = historicalATR,
                        Similarity = similarity,
                        Outcome = outcome
                    });
                }

                // 3. Candidato (>= SimilarityThreshold) — invariato
                if (similarity >= dynamicSimilarityThreshold)
                {
                    // Valuta bonus continuità prima di processare
                    bool isPersistent = State.LastScanPositions.Contains(pos);
                    ProcessCandidate(pos, similarity, lastClosed, isPersistent);
                }
            }

            // 4. TopN: ordina per similarità decrescente, tieni solo i migliori
            var topEntries = aftermathEntries
                .OrderByDescending(e => e.Similarity)
                .Take(AftermathTopN)
                .ToList();

            // 5. Registra SOLO i top entries nell'aftermath globale
            foreach (var entry in topEntries)
            {
                RecordAftermath(entry.Outcome, entry.StartBar,
                                entry.Count, entry.HistoricalATR,
                                entry.Similarity);
            }

            // 6. Pattern Library check — invariato
            foreach (var pattern in PatternDefinitions)
            {
                if (pattern.CheckPattern(Bars, lastClosed))
                {
                    ProcessPatternCandidate(pattern, lastClosed);
                }
            }

            // 7. Calcola conferma storica globale (percentuali)
            ProcessGlobalConfirmation();

            // 8. Calcola SL/TP dalle occorrenze
            CalculateSLTP();

            // Registra timestamp ultimo scan
            State.LastFullScanTime = Server.TimeInUtc;
        }

        // ============================================================
        // 11. AFTERMATH — CLASSIFICAZIONE A TRE ESITI
        // ============================================================

        // USA L'ATR STORICO della posizione, non l'ATR corrente.
        // Ogni movimento viene giudicato con la volatilita di quando e successo.

        private AftermathOutcome ClassifyAftermath(int startBar, int count, double atr)
        {
            double entryPrice = Bars.ClosePrices[startBar - 1];
            double maxUp = 0, maxDown = 0;

            for (int i = 0; i < count; i++)
            {
                int bar = startBar + i;
                if (bar >= Bars.Count) break;
                double diffHigh = Bars.HighPrices[bar] - entryPrice;
                double diffLow = entryPrice - Bars.LowPrices[bar];
                maxUp = Math.Max(maxUp, diffHigh);
                maxDown = Math.Max(maxDown, diffLow);
            }

            double threshold = atr * MovementATRMultiplier;
            if (threshold < 0.0001) return AftermathOutcome.Range;

            if (maxUp >= threshold && maxUp > maxDown)
                return AftermathOutcome.Long;
            if (maxDown >= threshold && maxDown > maxUp)
                return AftermathOutcome.Short;
            return AftermathOutcome.Range;
        }

        private void RecordAftermath(AftermathOutcome outcome, int startBar,
                                     int count, double atr, double similarity)
        {
            double entryPrice = Bars.ClosePrices[startBar - 1];

            // Peso ponderato: un match a similarita 92 conta 0.92,
            // uno a similarita 51 conta 0.51.
            // TotalOccurrences e sempre +1 intero (non pesato),
            // serve SOLO per il gate MinOccurrences.
            double weight = similarity / 100.0;
            GlobalAftermath.TotalOccurrences++;

            if (outcome == AftermathOutcome.Long)
            {
                GlobalAftermath.WeightedCountLong += weight;
                double drawdown = 0;
                double profit = 0;
                for (int i = 0; i < count; i++)
                {
                    int bar = startBar + i;
                    if (bar >= Bars.Count) break;
                    drawdown = Math.Max(drawdown, entryPrice - Bars.LowPrices[bar]);
                    profit = Math.Max(profit, Bars.HighPrices[bar] - entryPrice);
                }
                GlobalAftermath.DrawdownsLong.Add(drawdown);
                GlobalAftermath.ProfitsLong.Add(profit);
            }
            else if (outcome == AftermathOutcome.Short)
            {
                GlobalAftermath.WeightedCountShort += weight;
                double drawdown = 0;
                double profit = 0;
                for (int i = 0; i < count; i++)
                {
                    int bar = startBar + i;
                    if (bar >= Bars.Count) break;
                    drawdown = Math.Max(drawdown, Bars.HighPrices[bar] - entryPrice);
                    profit = Math.Max(profit, entryPrice - Bars.LowPrices[bar]);
                }
                GlobalAftermath.DrawdownsShort.Add(drawdown);
                GlobalAftermath.ProfitsShort.Add(profit);
            }
            else // Range
            {
                GlobalAftermath.WeightedCountRange += weight;
                double rangeHigh = double.MinValue;
                double rangeLow = double.MaxValue;
                bool hasData = false;
                for (int i = 0; i < count; i++)
                {
                    int bar = startBar + i;
                    if (bar >= Bars.Count) break;
                    hasData = true;
                    rangeHigh = Math.Max(rangeHigh, Bars.HighPrices[bar]);
                    rangeLow = Math.Min(rangeLow, Bars.LowPrices[bar]);
                }
                if (hasData)
                {
                    GlobalAftermath.RangeHighs.Add(rangeHigh - entryPrice);
                    GlobalAftermath.RangeLows.Add(entryPrice - rangeLow);
                }
            }
        }

        // ============================================================
        // 12. CONFERMA STORICA — CALCOLO GLOBALE
        // ============================================================

        // Calcola le percentuali dei tre esiti.
        // NON applica penalita. Le percentuali alimentano il contesto
        // direzionale tramite AftermathBias (Sezione 9).
        //
        // Le percentuali sono PONDERATE per similarita: un match a 92 pesa
        // 0.92, uno a 51 pesa 0.51. MinOccurrences e confrontato con
        // TotalOccurrences (intero non pesato).

        private void ProcessGlobalConfirmation()
        {
            // Gate: usa TotalOccurrences (intero non pesato)
            if (GlobalAftermath.TotalOccurrences < MinOccurrences)
            {
                State.PctLong = 0;
                State.PctShort = 0;
                State.PctRange = 0;
                return;
            }

            // Somma pesi ponderati
            double total = GlobalAftermath.WeightedCountLong +
                           GlobalAftermath.WeightedCountShort +
                           GlobalAftermath.WeightedCountRange;

            if (total < 0.0001)
            {
                State.PctLong = 0;
                State.PctShort = 0;
                State.PctRange = 0;
                return;
            }

            double rawPctLong  = GlobalAftermath.WeightedCountLong  / total * 100;
            double rawPctShort = GlobalAftermath.WeightedCountShort / total * 100;
            double rawPctRange = GlobalAftermath.WeightedCountRange / total * 100;

            // Smoothing Context (SMA 3)
            State.PctLongHistory.Add(rawPctLong);
            State.PctShortHistory.Add(rawPctShort);
            if (State.PctLongHistory.Count > 3) State.PctLongHistory.RemoveAt(0);
            if (State.PctShortHistory.Count > 3) State.PctShortHistory.RemoveAt(0);

            State.PctLong = State.PctLongHistory.Average();
            State.PctShort = State.PctShortHistory.Average();
            State.PctRange = rawPctRange; // Range non necessita smoothing critico
        }

        // ============================================================
        // 13. PROCESSAMENTO CANDIDATO STORICO
        // ============================================================

        private void ProcessCandidate(int scanPos, double similarity, int lastClosed, bool isPersistent = false)
        {
            int matchEnd = scanPos + AnalysisWindow;

            // Filtro cloni: se esiste gia un candidato entro CloneDistance,
            // seleziona il PIU VICINO (non il primo trovato nella lista)
            var closest = ActiveCandidates
                .Where(e => e.Source == CandidateSource.Historical &&
                            Math.Abs(e.HistoryPosition - scanPos) <= CloneDistance)
                .OrderBy(e => Math.Abs(e.HistoryPosition - scanPos))
                .FirstOrDefault();
            if (closest != null)
            {
                int distance = Math.Abs(closest.HistoryPosition - scanPos);

                if (distance == 0)
                {
                    // Exact re-find: aggiorna SEMPRE similarita e ghost
                    // (le ghost vanno riscalate al prezzo corrente ogni volta)
                    var newGhosts = BuildAndScaleGhosts(
                        matchEnd, DefaultGhostCount, lastClosed);
                    if (newGhosts.Count == 0)
                    {
                        closest.IsActive = false;
                        return;
                    }
                    
                    // Ritorna le vecchie ghost al pool prima di sovrascrivere
                    foreach (var g in closest.GhostCandles)
                        _ghostPool.Return(g);

                    closest.InitialSimilarity = similarity;
                    closest.GhostCandles = newGhosts;
                    closest.CandlesElapsed = 0;
                    closest.RealtimeScore = RT_START_VALUE;
                }
                else if (similarity > closest.InitialSimilarity)
                {
                    // Clone vicino: aggiorna solo se similarita migliore
                    var newGhosts = BuildAndScaleGhosts(
                        matchEnd, DefaultGhostCount, lastClosed);
                    if (newGhosts.Count == 0)
                    {
                        closest.IsActive = false;
                        return;
                    }

                    // Ritorna le vecchie ghost al pool prima di sovrascrivere
                    foreach (var g in closest.GhostCandles)
                        _ghostPool.Return(g);

                    closest.InitialSimilarity = similarity;
                    closest.GhostCandles = newGhosts;
                    closest.CandlesElapsed = 0;
                    closest.RealtimeScore = RT_START_VALUE;
                }
                closest.WasFoundInLastScan = true;
                RecalculateConfidence(closest);
                return;
            }

            // Costruisci ghost
            var ghosts = BuildAndScaleGhosts(matchEnd, DefaultGhostCount, lastClosed);
            if (ghosts.Count == 0) return;

            // PRIMA classifica — determina tipo PRIMA del clarity filter
            double ghostNetMove = ghosts.Last().Close - ghosts.First().Open;
            bool isBuy = ghostNetMove > 0;
            double ghostRange = ghosts.Max(g => g.High) - ghosts.Min(g => g.Low);
            double ghostAbsMove = Math.Abs(ghostNetMove);
            AftermathOutcome predicted;

            if (ghostRange < 0.0001)
                predicted = AftermathOutcome.Range;
            else if (ghostAbsMove / ghostRange < ClarityThreshold)
                predicted = AftermathOutcome.Range;
            else
                predicted = isBuy ? AftermathOutcome.Long : AftermathOutcome.Short;

            // POI clarity filter — solo se NON e range
            // Un candidato range ha ghost choppy per definizione
            if (predicted != AftermathOutcome.Range && !PassesClarityFilter(ghosts))
                return;

            // NOTA: il caso "exact re-find" (stessa posizione esatta) e ora
            // gestito dal clone filter sopra (distance == 0). Non serve un
            // blocco separato perche CloneDistance >= 1 cattura sempre dist 0.

            // Nuovo candidato dal pool
            var candidate = _candidatePool.Get();
            candidate.HistoryPosition = scanPos;
            candidate.InitialSimilarity = similarity;
            candidate.RealtimeScore = RT_START_VALUE;
            candidate.Source = CandidateSource.Historical;
            candidate.PatternName = $"Hist pos {scanPos}";
            candidate.IsActive = true;
            candidate.IsBuy = isBuy;
            candidate.PredictedOutcome = predicted;
            candidate.SuccessRate = -1;       // non calcolabile per storici
            candidate.GhostCandles = ghosts;
            candidate.EntryTime = Server.TimeInUtc;
            candidate.HourSlot = GetCurrentHourSlot();
            candidate.WasFoundInLastScan = isPersistent;
            candidate.CandlesElapsed = 0;

            RecalculateConfidence(candidate);
            ActiveCandidates.Add(candidate);
        }

        // ============================================================
        // 14. GHOST CANDLES — COSTRUZIONE E SCALING
        // ============================================================

        // Scaling PROPORZIONALE (ratio-based, non assoluto) + VOLATILITY SCALING.
        // Oltre allo scaling per rapporto prezzo, i delta OHLC vengono moltiplicati
        // per il rapporto tra ATR corrente e ATR storico del match.

        private List<GhostCandle> BuildAndScaleGhosts(int matchEnd, int ghostCount,
                                                       int lastClosed)
        {
            var ghosts = new List<GhostCandle>();
            double anchorPrice = Bars.ClosePrices[lastClosed];
            double historyClose = Bars.ClosePrices[matchEnd - 1];

            if (historyClose < 0.0001) return ghosts;

            // Volatility Scaling: rapporto ATR corrente / ATR storico
            double currentATR = CalcolaATR(lastClosed);
            double historicalATR = CalcolaATR(matchEnd - 1);
            double volRatio = (historicalATR > 0.0001)
                ? currentATR / historicalATR
                : 1.0;

            // Calcolo tempo base con guard per indice fuori range
            var baseTime = (lastClosed + 1 < Bars.Count)
                ? Bars.OpenTimes[lastClosed + 1]
                : Bars.OpenTimes[lastClosed] + TimeSpan.FromMinutes(_barPeriodMinutes);

            for (int i = 0; i < ghostCount; i++)
            {
                int srcBar = matchEnd + i;
                if (srcBar >= Bars.Count) break;

                double deltaOpen  = (Bars.OpenPrices[srcBar]  / historyClose) - 1.0;
                double deltaHigh  = (Bars.HighPrices[srcBar]  / historyClose) - 1.0;
                double deltaLow   = (Bars.LowPrices[srcBar]   / historyClose) - 1.0;
                double deltaClose = (Bars.ClosePrices[srcBar]  / historyClose) - 1.0;

                var ghost = _ghostPool.Get();
                ghost.Open  = anchorPrice * (1.0 + deltaOpen  * volRatio);
                ghost.High  = anchorPrice * (1.0 + deltaHigh  * volRatio);
                ghost.Low   = anchorPrice * (1.0 + deltaLow   * volRatio);
                ghost.Close = anchorPrice * (1.0 + deltaClose * volRatio);
                ghost.ProjectedTime = baseTime + TimeSpan.FromMinutes(_barPeriodMinutes * i);
                ghost.IsBullish = Bars.ClosePrices[srcBar] > Bars.OpenPrices[srcBar];

                ghosts.Add(ghost);
            }

            return ghosts;
        }

        private bool PassesClarityFilter(List<GhostCandle> ghosts)
        {
            if (ghosts.Count == 0) return false;
            double totalMovement = Math.Abs(ghosts.First().Close - ghosts.First().Open);
            for (int i = 1; i < ghosts.Count; i++)
                totalMovement += Math.Abs(ghosts[i].Close - ghosts[i - 1].Close);
            double netMovement = Math.Abs(ghosts.Last().Close - ghosts.First().Open);
            if (totalMovement < 0.0001) return false;
            return (netMovement / totalMovement) >= ClarityThreshold;
        }

        // NOTA: il clarity filter viene chiamato SOLO per candidati
        // direzionali (LONG/SHORT). I candidati RANGE lo bypassano
        // perche hanno movimento netto basso per definizione.
        // Vedi ProcessCandidate per l'ordine corretto.

        // ============================================================
        // 15. CONFIDENCE SCORE — FORMULA
        // ============================================================

        // Il confidence e PURO: dipende solo dalla similarita iniziale e
        // dal punteggio real-time. Nessun bias, nessuna penalita, nessun
        // contesto. Il contesto influenza la gara tramite EffectiveConfidence
        // (Sezione 9), non tramite il confidence.

        private void RecalculateConfidence(Candidate c)
        {
            double rawConfidence =
                (c.InitialSimilarity * CONFIDENCE_WEIGHT_IS) +
                (c.RealtimeScore * CONFIDENCE_WEIGHT_RT);

            // Bonus pattern library
            if (c.Source == CandidateSource.PatternLibrary)
                rawConfidence += BonusPatternLibrary;

            // Clamp 0-100
            c.ConfidenceScore = Math.Max(0, Math.Min(100, rawConfidence));
        }

        // ============================================================
        // 16. RIVALUTAZIONE 15 SECONDI — BuildFingerprintFromGhost
        // ============================================================

        private CandleFingerprint BuildFingerprintFromGhost(GhostCandle ghost)
        {
            double range = ghost.High - ghost.Low;
            if (range < 0.0001)
                return new CandleFingerprint(); // doji

            double body = Math.Abs(ghost.Close - ghost.Open);
            return new CandleFingerprint
            {
                Direction = (ghost.Close > ghost.Open) ? 1.0 :
                            (ghost.Close < ghost.Open) ? -1.0 : 0.0,
                BodyRatio = body / range,
                ClosePosition = (ghost.Close - ghost.Low) / range,
                UpperWickRatio = (ghost.High - Math.Max(ghost.Open, ghost.Close)) / range,
                LowerWickRatio = (Math.Min(ghost.Open, ghost.Close) - ghost.Low) / range,
                VolumeRelative = 1.0,   // neutro per ghost
                RangeRelative = 1.0     // neutro per ghost
            };
        }

        // ============================================================
        // 18. ELIMINAZIONE E RESET
        // ============================================================

        private void EliminateBelowThreshold()
        {
            var toRemove = new List<Candidate>();
            foreach (var c in ActiveCandidates)
            {
                if (!c.IsActive || c.ConfidenceScore < SurvivalThreshold)
                {
                    c.IsActive = false;
                    toRemove.Add(c);
                }
            }

            foreach (var c in toRemove)
            {
                ActiveCandidates.Remove(c);
                foreach (var g in c.GhostCandles)
                    _ghostPool.Return(g);
                _candidatePool.Return(c);
            }
        }

        private void HandleReset()
        {
            foreach (var c in ActiveCandidates)
            {
                foreach (var g in c.GhostCandles)
                    _ghostPool.Return(g);
                _candidatePool.Return(c);
            }
            ActiveCandidates.Clear();
            
            if (State.ActiveSignal != null)
            {
                foreach (var g in State.ActiveSignal.SignalGhosts)
                    _ghostPool.Return(g);
                State.ActiveSignal.SignalGhosts.Clear();
                State.ActiveSignal.IsValid = false;
            }

            State.CurrentFavorite = null;
            State.IsInResetState = true;
            ClearGhostRendering();
        }

        // NOTA: GlobalAftermath NON viene resettata qui perche
        // ExecuteFullScan() la resetta all'inizio del prossimo scan.

        // ============================================================
        // 19. FAVORITO E SEGNALE
        // ============================================================

        private void UpdateFavorite()
        {
            if (ActiveCandidates.Count == 0)
            {
                State.CurrentFavorite = null;
                return;
            }

            // Ordina per EffectiveConfidence (che include il contesto direzionale)
            var sorted = ActiveCandidates
                .Where(c => c.IsActive)
                .OrderByDescending(c => GetEffectiveConfidence(c))
                .ToList();

            if (sorted.Count == 0)
            {
                State.CurrentFavorite = null;
                return;
            }

            // === CLUSTERING / CONSENSUS FILTERING ===
            // Analizziamo i top 5 per vedere se c'è un consenso forte su una direzione.
            // Se il "best" è isolato e gli altri 4 dicono l'opposto, dubitiamo del best.
            int clusterSize = Math.Min(5, sorted.Count);
            var topCluster = sorted.Take(clusterSize).ToList();
            
            double buyWeight = topCluster.Where(c => c.IsBuy).Sum(c => GetEffectiveConfidence(c));
            double sellWeight = topCluster.Where(c => !c.IsBuy).Sum(c => GetEffectiveConfidence(c));
            
            var best = sorted.First();
            
            // Se la direzione dominante nel cluster ha un peso significativamente maggiore (> 60%),
            // e il "best" è della direzione opposta, potremmo voler switchare al miglior candidato della direzione dominante.
            double totalWeight = buyWeight + sellWeight;
            if (totalWeight > 0)
            {
                bool consensusBuy = (buyWeight / totalWeight) > 0.70;
                bool consensusSell = (sellWeight / totalWeight) > 0.70;
                
                if (consensusBuy && !best.IsBuy)
                {
                    best = sorted.FirstOrDefault(c => c.IsBuy) ?? best;
                }
                else if (consensusSell && best.IsBuy)
                {
                    best = sorted.FirstOrDefault(c => !c.IsBuy) ?? best;
                }
            }

            var current = State.CurrentFavorite;

            // Se la differenza tra il primo e il secondo è < 2%, 
            // mantieni quello che ha la direzione del segnale attuale (se esiste)
            if (sorted.Count > 1 && State.ActiveSignal != null && State.ActiveSignal.IsValid)
            {
                var second = sorted[1];
                double bestEC = GetEffectiveConfidence(best);
                double secondEC = GetEffectiveConfidence(second);
                
                // Regola del 2%: favorisci la stabilità se lo scarto è minimo
                if ((bestEC - secondEC) < (bestEC * 0.02))
                {
                    bool bestMatches = (best.IsBuy == State.ActiveSignal.IsBuy);
                    bool secondMatches = (second.IsBuy == State.ActiveSignal.IsBuy);
                    
                    if (!bestMatches && secondMatches)
                    {
                        best = second;
                    }
                }
            }

            // Se c'è già un favorito, applica inerzia per cambio direzione
            if (current != null && current.IsActive)
            {
                double bestEC_final = GetEffectiveConfidence(best);
                double currentEC = GetEffectiveConfidence(current);

                bool sameDirection = (best.PredictedOutcome == current.PredictedOutcome);

                if (!sameDirection)
                {
                    // === Dinamismo Volatilità su Inerzia ===
                    double currentATR = CalcolaATR(Bars.Count - 1);
                    double avgATR = 0;
                    int lookback = 50;
                    if (Bars.Count > lookback + 1)
                    {
                        for (int i = 0; i < lookback; i++) avgATR += CalcolaATR(Bars.Count - 1 - i);
                        avgATR /= (double)lookback;
                    }
                    else avgATR = currentATR;
                    double volRatio = (avgATR > 0) ? currentATR / avgATR : 1.0;
                    
                    // In alta volatilità (espansione), aumentiamo l'inerzia per evitare di essere "shakerati"
                    // In bassa volatilità (compressione), l'inerzia può essere standard
                    double dynamicInertia = InertiaPenalty * (1.0 + (volRatio - 1.0) * VolatilitySensitivity);
                    dynamicInertia = Math.Max(2.0, dynamicInertia);

                    // Direzione DIVERSA: il nuovo deve superare il favorito attuale
                    // di un margine significativo (dynamicInertia * 2) per evitare oscillazioni
                    if (bestEC_final < currentEC + (dynamicInertia * 2.0))
                    {
                        best = current; // Rigetta l'inversione, resta sul vecchio
                    }
                }
            }

            State.CurrentFavorite = best;
            State.IsInResetState = false;
        }

        private void TryCreateSignal()
        {
            var fav = State.CurrentFavorite;
            if (fav == null) return;
            if (State.ActiveSignal != null && State.ActiveSignal.IsValid) return;

            // Soglia minima confidence (confidence REALE, non effective)
            if (fav.ConfidenceScore < MinSignalConfidence) return;

            // --- Filtro HTF (Heikin Ashi) ---
            if (fav.PredictedOutcome != AftermathOutcome.Range && !IsHtfTrendAligned(fav.IsBuy))
            {
                // Solo log nel pannello o debug se vuoi
                return;
            }

            // Annullamento combinato — ESCLUDI i Range
            int countBuy = ActiveCandidates.Count(c =>
                c.PredictedOutcome != AftermathOutcome.Range && c.IsBuy);
            int countSell = ActiveCandidates.Count(c =>
                c.PredictedOutcome != AftermathOutcome.Range && !c.IsBuy);
            int totalDirectional = countBuy + countSell;
            int effectiveCancellation = Math.Max(CancellationDiff,
                (int)(totalDirectional * CancellationRatio));
            if (Math.Abs(countBuy - countSell) < effectiveCancellation &&
                fav.PredictedOutcome != AftermathOutcome.Range)
                return;

            // Blocca creazione se SL non calcolato (evita SL = entryPrice)
            if (fav.PredictedOutcome == AftermathOutcome.Long && State.CalculatedSLLong <= 0) return;
            if (fav.PredictedOutcome == AftermathOutcome.Short && State.CalculatedSLShort <= 0) return;
            // Guard difensivo: blocca creazione se TP non calcolato (evita TP = entryPrice, rr = 0)
            if (fav.PredictedOutcome == AftermathOutcome.Long && State.CalculatedTPLong <= 0) return;
            if (fav.PredictedOutcome == AftermathOutcome.Short && State.CalculatedTPShort <= 0) return;
            if (fav.PredictedOutcome == AftermathOutcome.Range &&
                (State.CalculatedRangeHigh <= 0 || State.CalculatedRangeLow <= 0)) return;

            // Usa il prezzo live: LastValue funziona correttamente sia a
            // chiusura barra (close della barra appena chiusa) sia mid-bar
            // (prezzo corrente). Evita discrepanze SL/TP quando TryCreateSignal
            // viene invocato da OnTimer durante la formazione di una nuova barra.
            double entryPrice = Bars.ClosePrices.LastValue;

            // SL/TP: converti distanze in livelli di prezzo
            double slLevel = 0;
            double tpLevel = 0;
            double rr = 0;
            double rangeHigh = 0;
            double rangeLow = 0;

            double worstCaseLevel = 0;

            if (fav.PredictedOutcome == AftermathOutcome.Long)
            {
                // BUY: SL sotto, TP sopra, WC sotto SL
                slLevel = entryPrice - State.CalculatedSLLong;
                tpLevel = entryPrice + State.CalculatedTPLong;
                if (State.CalculatedSLLong > 0)
                    rr = State.CalculatedTPLong / State.CalculatedSLLong;
                worstCaseLevel = State.WorstCaseLong > 0
                    ? entryPrice - State.WorstCaseLong
                    : 0;
            }
            else if (fav.PredictedOutcome == AftermathOutcome.Short)
            {
                // SELL: SL sopra, TP sotto, WC sopra SL
                slLevel = entryPrice + State.CalculatedSLShort;
                tpLevel = entryPrice - State.CalculatedTPShort;
                if (State.CalculatedSLShort > 0)
                    rr = State.CalculatedTPShort / State.CalculatedSLShort;
                worstCaseLevel = State.WorstCaseShort > 0
                    ? entryPrice + State.WorstCaseShort
                    : 0;
            }
            else // Range
            {
                rangeHigh = State.CalculatedRangeHigh;
                rangeLow = State.CalculatedRangeLow;
                // WorstCaseLevel = 0 per RANGE (non applicabile)
            }

            // Crea segnale
            if (State.ActiveSignal != null)
            {
                foreach (var g in State.ActiveSignal.SignalGhosts)
                    _ghostPool.Return(g);
                State.ActiveSignal.SignalGhosts.Clear();
            }

            State.ActiveSignal = new FixedSignalSet
            {
                IsBuy = fav.IsBuy,
                Outcome = fav.PredictedOutcome,
                SignalGhosts = fav.GhostCandles
                    .Select(g => DeepCopyGhost(g)).ToList(),
                FinalConfidence = fav.ConfidenceScore,  // confidence REALE
                GhostsRemaining = fav.GhostCandles.Count,
                StopLossLevel = slLevel,
                TakeProfitLevel = tpLevel,
                RiskReward = rr,
                WorstCaseLevel = worstCaseLevel,
                RangeHigh = rangeHigh,
                RangeLow = rangeLow,
                IsValid = true,
                SourceInfo = fav.PatternName
            };
            State.SignalCreationTime = Server.TimeInUtc; // Salva tempo creazione

            // === BOT: Esegui ordine di mercato per il segnale appena creato ===
            try
            {
                TryExecuteTrade();
            }
            catch (Exception ex)
            {
                Print($"[VIT BOT] ERRORE in TryExecuteTrade: {ex.Message}\n{ex.StackTrace}");
            }
        }

        // ============================================================
        // 19.5 BOT — ESECUZIONE ORDINE MARKET
        // ============================================================

        private void TryExecuteTrade()
        {
            if (!EnableTrading) return;

            // --- Filtro Orario Trading ---
            int hour = Server.TimeInUtc.Hour;
            if (hour < TradingStartHour || hour >= TradingEndHour)
            {
                Print("[VIT BOT] Skip apertura: fuori orario trading");
                return;
            }

            // --- Filtro Spread ---
            double spreadPips = Symbol.Spread / Symbol.PipSize;
            if (spreadPips > MaxSpreadPips)
            {
                Print($"[VIT BOT] Skip apertura: spread troppo alto ({spreadPips:F1} pips)");
                return;
            }

            var sig = State.ActiveSignal;
            if (sig == null || !sig.IsValid) return;

            // Solo segnali direzionali: skip RANGE
            if (sig.Outcome == AftermathOutcome.Range)
            {
                Print("[VIT BOT] Skip apertura: segnale RANGE non tradato");
                // Se c'è una posizione aperta, comunque chiudila (segnale direzionale è sparito)
                SyncPositionWithSignal();
                return;
            }

            // Guard difensivo: SL/TP devono essere valorizzati
            if (sig.StopLossLevel <= 0 || sig.TakeProfitLevel <= 0)
            {
                Print($"[VIT BOT] Skip apertura: SL/TP non validi (SL={sig.StopLossLevel} TP={sig.TakeProfitLevel})");
                return;
            }

            // Sincronizza PRIMA: se c'è una posizione di direzione opposta, la chiude
            // (permette il flip BUY→SELL e viceversa). Dopo questa chiamata, Positions.FindAll
            // non dovrebbe contenere posizioni di direzione opposta al segnale.
            SyncPositionWithSignal();

            // Se dopo la sync esiste ancora una posizione (stessa direzione del segnale),
            // vuol dire che è già allineata: niente da aprire
            var positions = Positions.FindAll(TradeLabel, SymbolName);
            if (positions.Length > 0)
            {
                _pendingOpenSignal = null;
                _pendingOpenAttempts = 0;
                return;
            }

            // Prova ad aprire
            if (OpenPosition(sig))
            {
                _pendingOpenSignal = null;
                _pendingOpenAttempts = 0;
            }
            else
            {
                // Apertura fallita: memorizza per retry al prossimo OnTick
                _pendingOpenSignal = sig;
                _pendingOpenAttempts++;
                Print($"[VIT BOT] Apertura fallita al tentativo {_pendingOpenAttempts}, retry al prossimo tick");
            }
        }

        private bool OpenPosition(FixedSignalSet sig)
        {
            // Converti lotti richiesti in unità broker-specific e normalizza
            double volumeInUnits = Symbol.QuantityToVolumeInUnits(TradeVolumeLots);
            volumeInUnits = Symbol.NormalizeVolumeInUnits(volumeInUnits, RoundingMode.ToNearest);
            if (volumeInUnits < Symbol.VolumeInUnitsMin)
                volumeInUnits = Symbol.VolumeInUnitsMin;

            TradeType tradeType = sig.IsBuy ? TradeType.Buy : TradeType.Sell;
            // Prezzo di riferimento live: Ask per BUY (lo paghi), Bid per SELL (lo ricevi)
            double refPrice = sig.IsBuy ? Symbol.Ask : Symbol.Bid;

            // Calcola distanza SL/TP in pip (ExecuteMarketOrder richiede pips)
            double slPips, tpPips;
            if (sig.IsBuy)
            {
                if (sig.StopLossLevel >= refPrice || sig.TakeProfitLevel <= refPrice)
                {
                    Print($"[VIT BOT] Skip BUY: SL/TP non coerenti " +
                          $"(SL={sig.StopLossLevel:F5} TP={sig.TakeProfitLevel:F5} Ask={refPrice:F5})");
                    return false;
                }
                slPips = (refPrice - sig.StopLossLevel) / Symbol.PipSize;
                tpPips = (sig.TakeProfitLevel - refPrice) / Symbol.PipSize;
            }
            else
            {
                if (sig.StopLossLevel <= refPrice || sig.TakeProfitLevel >= refPrice)
                {
                    Print($"[VIT BOT] Skip SELL: SL/TP non coerenti " +
                          $"(SL={sig.StopLossLevel:F5} TP={sig.TakeProfitLevel:F5} Bid={refPrice:F5})");
                    return false;
                }
                slPips = (sig.StopLossLevel - refPrice) / Symbol.PipSize;
                tpPips = (refPrice - sig.TakeProfitLevel) / Symbol.PipSize;
            }

            slPips = Math.Round(slPips, 1);
            tpPips = Math.Round(tpPips, 1);

            if (slPips <= 0 || tpPips <= 0)
            {
                Print($"[VIT BOT] Skip: distanze non valide dopo round (SL={slPips}p TP={tpPips}p)");
                return false;
            }

            var result = ExecuteMarketOrder(tradeType, SymbolName, volumeInUnits,
                                            TradeLabel, slPips, tpPips);

            if (result.IsSuccessful && result.Position != null)
            {
                Print($"[VIT BOT] APERTA {tradeType} vol={volumeInUnits} " +
                      $"entry={result.Position.EntryPrice:F5} " +
                      $"SL={sig.StopLossLevel:F5} ({slPips:F1}p) " +
                      $"TP={sig.TakeProfitLevel:F5} ({tpPips:F1}p) " +
                      $"pattern={sig.SourceInfo} conf={sig.FinalConfidence:F0} RR={sig.RiskReward:F2}");
                return true;
            }
            else
            {
                string err = result != null && result.Error.HasValue
                    ? result.Error.Value.ToString()
                    : "result null / error null";
                Print($"[VIT BOT] Ordine FALLITO: {err}");
                return false;
            }
        }

        // Chiude posizioni del bot se il segnale è scomparso, invalidato, o di direzione opposta.
        // Gestisce sia "scomparsa del segnale" (chiusura secca) sia "flip direzione" (chiude vecchia
        // prima che OpenPosition apra la nuova). A prescindere da profit/loss della posizione.
        private void SyncPositionWithSignal()
        {
            var positions = Positions.FindAll(TradeLabel, SymbolName);
            if (positions.Length == 0) return;

            var sig = State.ActiveSignal;
            // "Segnale operativo" = non null + valido + direzionale (non range)
            bool signalOperational = sig != null && sig.IsValid && sig.Outcome != AftermathOutcome.Range;

            foreach (var pos in positions)
            {
                // Caso 1: nessun segnale operativo → segnale scomparso/invalidato → chiudi
                if (!signalOperational)
                {
                    Print($"[VIT BOT] Chiusura {pos.TradeType}: segnale scomparso o invalidato " +
                          $"(entry={pos.EntryPrice:F5} pnl={pos.NetProfit:F2})");
                    var cr = ClosePosition(pos);
                    if (!cr.IsSuccessful)
                    {
                        string err = cr.Error.HasValue ? cr.Error.Value.ToString() : "error null";
                        Print($"[VIT BOT] ATTENZIONE: chiusura fallita: {err}. Verrà ritentata al prossimo ciclo.");
                    }
                    continue;
                }

                // Caso 2: segnale operativo ma direzione opposta → flip → chiudi (la nuova apertura
                // viene gestita da TryExecuteTrade dopo questa chiamata, o al prossimo ciclo)
                bool posBuy = pos.TradeType == TradeType.Buy;
                if (sig.IsBuy != posBuy)
                {
                    Print($"[VIT BOT] Chiusura flip {pos.TradeType}: nuovo segnale={(sig.IsBuy ? "BUY" : "SELL")} " +
                          $"(entry={pos.EntryPrice:F5} pnl={pos.NetProfit:F2})");
                    var cr = ClosePosition(pos);
                    if (!cr.IsSuccessful)
                    {
                        string err = cr.Error.HasValue ? cr.Error.Value.ToString() : "error null";
                        Print($"[VIT BOT] ATTENZIONE: chiusura flip fallita: {err}. Verrà ritentata al prossimo ciclo.");
                    }
                    continue;
                }

                // Caso 3: stessa direzione, posizione allineata
                // === Novità: aggiornamento dinamico SL/TP ===
                if (EnableTrading && sig.StopLossLevel > 0 && sig.TakeProfitLevel > 0)
                {
                    double currentSL = pos.StopLoss ?? 0;
                    double currentTP = pos.TakeProfit ?? 0;

                    // Se i livelli del segnale differiscono dai livelli attuali della posizione
                    // di almeno 0.5 pip, aggiorna la posizione.
                    double threshold = Symbol.PipSize * 0.5;
                    bool slChanged = Math.Abs(currentSL - sig.StopLossLevel) > threshold;
                    bool tpChanged = Math.Abs(currentTP - sig.TakeProfitLevel) > threshold;

                    if (slChanged || tpChanged)
                    {
                        var result = ModifyPosition(pos, sig.StopLossLevel, sig.TakeProfitLevel);
                        if (!result.IsSuccessful)
                        {
                            Print($"[VIT BOT] Aggiornamento SL/TP fallito per {pos.Id}: {result.Error}");
                        }
                    }
                }
            }
        }

        // ============================================================
        // 20. SL/TP — CALCOLO DALLE OCCORRENZE
        // ============================================================

        private void CalculateSLTP()
        {
            State.CalculatedSLLong = 0;
            State.CalculatedTPLong = 0;
            State.CalculatedSLShort = 0;
            State.CalculatedTPShort = 0;
            State.CalculatedRangeHigh = 0;
            State.CalculatedRangeLow = 0;
            State.WorstCaseLong = 0;
            State.WorstCaseShort = 0;

            // Long SL/TP + Worst Case (se abbastanza occorrenze)
            if (GlobalAftermath.DrawdownsLong.Count >= MinOccurrences)
            {
                State.CalculatedSLLong = CalculatePercentile(
                    GlobalAftermath.DrawdownsLong, SLPercentile);
                State.CalculatedTPLong = CalculateMedian(
                    GlobalAftermath.ProfitsLong);
                State.WorstCaseLong = CalculatePercentile(
                    GlobalAftermath.DrawdownsLong, WorstCasePercentile);
            }

            // Short SL/TP + Worst Case (se abbastanza occorrenze)
            if (GlobalAftermath.DrawdownsShort.Count >= MinOccurrences)
            {
                State.CalculatedSLShort = CalculatePercentile(
                    GlobalAftermath.DrawdownsShort, SLPercentile);
                State.CalculatedTPShort = CalculateMedian(
                    GlobalAftermath.ProfitsShort);
                State.WorstCaseShort = CalculatePercentile(
                    GlobalAftermath.DrawdownsShort, WorstCasePercentile);
            }

            // Range (se abbastanza occorrenze) — guard simmetrico su entrambe le liste
            if (GlobalAftermath.RangeHighs.Count >= MinOccurrences &&
                GlobalAftermath.RangeLows.Count >= MinOccurrences)
            {
                double entryPrice = Bars.ClosePrices[Bars.Count - 2];
                double rh = entryPrice + CalculateMedian(GlobalAftermath.RangeHighs);
                double rl = entryPrice - CalculateMedian(GlobalAftermath.RangeLows);
                State.CalculatedRangeHigh = Math.Max(rh, rl);
                State.CalculatedRangeLow = Math.Min(rh, rl);
            }
        }

        // Sezione 20 — Helper Percentile/Mediana
        private double CalculatePercentile(List<double> values, int percentile)
        {
            if (values.Count == 0) return 0;
            percentile = Math.Max(0, Math.Min(100, percentile));
            var sorted = values.OrderBy(v => v).ToList();
            double rank = (percentile / 100.0) * (sorted.Count - 1);
            int lower = (int)Math.Floor(rank);
            int upper = (int)Math.Ceiling(rank);
            if (lower == upper) return sorted[lower];
            double fraction = rank - lower;
            return sorted[lower] + (sorted[upper] - sorted[lower]) * fraction;
        }

        private double CalculateMedian(List<double> values)
        {
            return CalculatePercentile(values, 50);
        }

        // Sezione 24 — Pattern Library (stub, pattern list vuota per ora)
        private void ProcessPatternCandidate(IPatternDefinition pattern, int lastClosed)
        {
            // Filtro duplicati: se esiste gia un candidato attivo
            // con lo stesso PatternName, aggiorna quello esistente
            var existing = ActiveCandidates.FirstOrDefault(c =>
                c.Source == CandidateSource.PatternLibrary &&
                c.PatternName == pattern.Name &&
                c.IsActive);

            if (existing != null)
            {
                double successRateUpd = CalculatePatternSuccessRate(
                    pattern, lastClosed, PatternConfirmDepth);
                if (successRateUpd >= 0)
                    existing.InitialSimilarity = successRateUpd;
                
                var newGhosts = pattern.GenerateGhosts(
                    Bars, lastClosed, Bars.ClosePrices[lastClosed]);

                // Guard: se le nuove ghost sono vuote, disattiva il candidato
                if (newGhosts == null || newGhosts.Count == 0)
                {
                    existing.IsActive = false;
                    return;
                }

                // Ritorna le vecchie ghost al pool prima di sovrascrivere
                foreach (var g in existing.GhostCandles)
                    _ghostPool.Return(g);

                existing.GhostCandles = newGhosts;

                // Ricalcola IsBuy e PredictedOutcome dalle nuove ghost
                double ghostNetMoveUpd = existing.GhostCandles.Last().Close - existing.GhostCandles.First().Open;
                existing.IsBuy = ghostNetMoveUpd > 0;
                double ghostRangeUpd = existing.GhostCandles.Max(g => g.High) - existing.GhostCandles.Min(g => g.Low);
                double ghostAbsMoveUpd = Math.Abs(ghostNetMoveUpd);
                if (ghostRangeUpd < 0.0001)
                    existing.PredictedOutcome = AftermathOutcome.Range;
                else if (ghostAbsMoveUpd / ghostRangeUpd < ClarityThreshold)
                    existing.PredictedOutcome = AftermathOutcome.Range;
                else
                    existing.PredictedOutcome = existing.IsBuy ? AftermathOutcome.Long : AftermathOutcome.Short;

                existing.CandlesElapsed = 0;
                existing.RealtimeScore = RT_START_VALUE;
                existing.WasFoundInLastScan = true;
                RecalculateConfidence(existing);
                return;
            }

            // Se non esiste, crea nuovo
            double successRate = CalculatePatternSuccessRate(
                pattern, lastClosed, PatternConfirmDepth);

            double patternIS;
            if (successRate >= 0)
                patternIS = successRate;
            else
                patternIS = SimilarityThreshold;

            var ghosts = pattern.GenerateGhosts(
                Bars, lastClosed, Bars.ClosePrices[lastClosed]);

            // Guard: candidato con zero ghost è inutile
            if (ghosts == null || ghosts.Count == 0) return;

            // Determina direzione dalle ghost (ghosts.Count > 0 garantito dal guard)
            double ghostNetMove = ghosts.Last().Close - ghosts.First().Open;
            bool isBuy = ghostNetMove > 0;
            double ghostRange = ghosts.Max(g => g.High) - ghosts.Min(g => g.Low);
            double ghostAbsMove = Math.Abs(ghostNetMove);
            AftermathOutcome predicted;

            if (ghostRange < 0.0001)
                predicted = AftermathOutcome.Range;
            else if (ghostAbsMove / ghostRange < ClarityThreshold)
                predicted = AftermathOutcome.Range;
            else
                predicted = isBuy ? AftermathOutcome.Long : AftermathOutcome.Short;

            var candidate = _candidatePool.Get();
            candidate.HistoryPosition = lastClosed;
            candidate.InitialSimilarity = patternIS;
            candidate.RealtimeScore = RT_START_VALUE;
            candidate.Source = CandidateSource.PatternLibrary;
            candidate.PatternName = pattern.Name;
            candidate.IsActive = true;
            candidate.IsBuy = isBuy;
            candidate.PredictedOutcome = predicted;
            candidate.SuccessRate = successRate;
            candidate.GhostCandles = ghosts;
            candidate.EntryTime = Server.TimeInUtc;
            candidate.HourSlot = GetCurrentHourSlot();
            candidate.WasFoundInLastScan = true;
            candidate.CandlesElapsed = 0;

            RecalculateConfidence(candidate);
            ActiveCandidates.Add(candidate);
        }

        // Sezione 24 — Calcolo success rate pattern
        private double CalculatePatternSuccessRate(IPatternDefinition pattern,
                                                    int currentEndIndex,
                                                    int depth)
        {
            int startPos = currentEndIndex - depth;
            if (startPos < pattern.RequiredCandles + AftermathCandles)
                startPos = pattern.RequiredCandles + AftermathCandles;

            int occurrences = 0;
            int successes = 0;
            bool patternIsBull = pattern.IsBullish; // cache: costante per chiamata

            for (int pos = startPos;
                 pos <= currentEndIndex - AftermathCandles;
                 pos++)
            {
                if (pattern.CheckPattern(Bars, pos))
                {
                    occurrences++;

                    int afterStart = pos + 1;
                    double historicalATR = CalcolaATR(afterStart);
                    double threshold = historicalATR * MovementATRMultiplier;
                    double entryPrice = Bars.ClosePrices[pos];

                    double maxMove = 0;
                    for (int i = 0; i < AftermathCandles; i++)
                    {
                        int bar = afterStart + i;
                        if (bar >= Bars.Count) break;
                        // Conta solo il movimento nella direzione attesa del pattern
                        double relevantMove = patternIsBull
                            ? Bars.HighPrices[bar] - entryPrice
                            : entryPrice - Bars.LowPrices[bar];
                        if (relevantMove > maxMove) maxMove = relevantMove;
                    }

                    if (maxMove >= threshold)
                        successes++;
                }
            }

            if (occurrences < MinOccurrences)
                return -1; // non abbastanza dati

            return (double)successes / occurrences * 100.0;
        }

        // ============================================================
        // PATTERN LIBRARY — CLASSI DEI PATTERN
        // ============================================================

        // ---- N.1 Bull Flag ----
        private class BullFlagPattern : PatternBase
        {
            public BullFlagPattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bull Flag";
            public override int RequiredCandles => 8;
            public override int GhostCandleCount => 5;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (!(localATR > 0)) return false;

                // 1. Impulso (c[0]-c[2]): C[2] - O[0] > 1.5 * localATR
                double impulse = C(bars, endIndex, 2) - O(bars, endIndex, 0);
                if (impulse <= 1.5 * localATR) return false;

                // Almeno 2 su 3 candele bullish
                int bullCount = 0;
                if (IsBull(bars, endIndex, 0)) bullCount++;
                if (IsBull(bars, endIndex, 1)) bullCount++;
                if (IsBull(bars, endIndex, 2)) bullCount++;
                if (bullCount < 2) return false;

                // 2. Consolidamento (c[3]-c[7]):
                // H[i] <= H[i-1] + 0.1 * localATR per i=4..7
                for (int i = 4; i <= 7; i++)
                {
                    if (H(bars, endIndex, i) > H(bars, endIndex, i - 1) + 0.1 * localATR)
                        return false;
                }
                // L[i] > O[0] + (C[2] - O[0]) * 0.5 per i=3..7
                double retraceLevel = O(bars, endIndex, 0) + impulse * 0.5;
                for (int i = 3; i <= 7; i++)
                {
                    if (L(bars, endIndex, i) <= retraceLevel) return false;
                }

                // 3. Range decrescente: range[7] < max(range[3], range[4])
                double r7 = Range(bars, endIndex, 7);
                double maxR34 = Math.Max(Range(bars, endIndex, 3), Range(bars, endIndex, 4));
                if (r7 >= maxR34) return false;

                // 4. C[7] > (consHigh + consLow) / 2
                double consHigh = double.MinValue;
                double consLow = double.MaxValue;
                for (int i = 3; i <= 7; i++)
                {
                    consHigh = Math.Max(consHigh, H(bars, endIndex, i));
                    consLow = Math.Min(consLow, L(bars, endIndex, i));
                }
                if (C(bars, endIndex, 7) <= (consHigh + consLow) / 2.0) return false;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                // Target: currentPrice + (C[2] - O[0]) — Caso A
                // Bull: C[2] > O[0] → positivo → target sopra currentPrice
                double target = currentPrice + (C(bars, endIndex, 2) - O(bars, endIndex, 0));
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.1 Bear Flag (bear esplicito) ----
        private class BearFlagPattern : PatternBase
        {
            public BearFlagPattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bear Flag";
            public override int RequiredCandles => 8;
            public override int GhostCandleCount => 5;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (!(localATR > 0)) return false;

                // 1. Impulso bear esplicito: O[0] - C[2] > 1.5 * localATR
                double impulse = O(bars, endIndex, 0) - C(bars, endIndex, 2);
                if (impulse <= 1.5 * localATR) return false;

                // Almeno 2 su 3 candele bearish
                int bearCount = 0;
                if (IsBear(bars, endIndex, 0)) bearCount++;
                if (IsBear(bars, endIndex, 1)) bearCount++;
                if (IsBear(bars, endIndex, 2)) bearCount++;
                if (bearCount < 2) return false;

                // 2. Consolidamento mirror:
                // L[i] >= L[i-1] - 0.1 * localATR per i=4..7 (lows non scendono)
                for (int i = 4; i <= 7; i++)
                {
                    if (L(bars, endIndex, i) < L(bars, endIndex, i - 1) - 0.1 * localATR)
                        return false;
                }
                // H[i] < livello 50% retrace per i=3..7
                // Retrace level = O[0] + (C[2] - O[0]) * 0.5 = (O[0] + C[2]) / 2
                double retraceLevel = O(bars, endIndex, 0) + (C(bars, endIndex, 2) - O(bars, endIndex, 0)) * 0.5;
                for (int i = 3; i <= 7; i++)
                {
                    if (H(bars, endIndex, i) >= retraceLevel) return false;
                }

                // 3. Range decrescente (uguale al bull)
                double r7 = Range(bars, endIndex, 7);
                double maxR34 = Math.Max(Range(bars, endIndex, 3), Range(bars, endIndex, 4));
                if (r7 >= maxR34) return false;

                // 4. C[7] < (consHigh + consLow) / 2 (close nella meta' inferiore)
                double consHigh = double.MinValue;
                double consLow = double.MaxValue;
                for (int i = 3; i <= 7; i++)
                {
                    consHigh = Math.Max(consHigh, H(bars, endIndex, i));
                    consLow = Math.Min(consLow, L(bars, endIndex, i));
                }
                if (C(bars, endIndex, 7) >= (consHigh + consLow) / 2.0) return false;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                // Target: currentPrice + (C[2] - O[0]) — formula identica, NON mirror
                // Bear: C[2] < O[0] → negativo → target sotto currentPrice
                double target = currentPrice + (C(bars, endIndex, 2) - O(bars, endIndex, 0));
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.2 Double Bottom ----
        private class DoubleBottomPattern : PatternBase
        {
            public DoubleBottomPattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Double Bottom";
            public override int RequiredCandles => 12;
            public override int GhostCandleCount => 5;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (!(localATR > 0)) return false;

                // 1. Bottom 1: min(L[0]..L[4])
                double low1 = double.MaxValue;
                int idx1 = 0;
                for (int i = 0; i <= 4; i++)
                {
                    double val = L(bars, endIndex, i);
                    if (val < low1) { low1 = val; idx1 = i; }
                }

                // 2. Bottom 2: min(L[7]..L[11]), vincolo idx2 <= 10
                double low2 = double.MaxValue;
                int idx2 = 7;
                for (int i = 7; i <= 11; i++)
                {
                    double val = L(bars, endIndex, i);
                    if (val < low2) { low2 = val; idx2 = i; }
                }
                if (idx2 > 10) return false;

                // 3. Livelli simili: |low1 - low2| < 0.3 * localATR
                if (Math.Abs(low1 - low2) >= 0.3 * localATR) return false;

                // 4. Neckline: max(H[idx1]..H[idx2])
                double neckline = double.MinValue;
                for (int i = idx1; i <= idx2; i++)
                {
                    neckline = Math.Max(neckline, H(bars, endIndex, i));
                }
                if (neckline <= Math.Max(low1, low2) + 0.5 * localATR) return false;

                // 5. Break: C[11] > neckline
                if (C(bars, endIndex, 11) <= neckline) return false;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                // Caso A: ricalcola neckline e low direttamente
                double low1 = double.MaxValue;
                int idx1 = 0;
                for (int i = 0; i <= 4; i++)
                {
                    double val = L(bars, endIndex, i);
                    if (val < low1) { low1 = val; idx1 = i; }
                }
                double low2 = double.MaxValue;
                int idx2 = 7;
                for (int i = 7; i <= 11; i++)
                {
                    double val = L(bars, endIndex, i);
                    if (val < low2) { low2 = val; idx2 = i; }
                }
                double neckline = double.MinValue;
                for (int i = idx1; i <= idx2; i++)
                {
                    neckline = Math.Max(neckline, H(bars, endIndex, i));
                }

                // Target: currentPrice + (neckline - min(low1, low2))
                double target = currentPrice + (neckline - Math.Min(low1, low2));
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.2 Double Top (mirror del Double Bottom) ----
        private class DoubleTopPattern : PatternBase
        {
            public DoubleTopPattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Double Top";
            public override int RequiredCandles => 12;
            public override int GhostCandleCount => 5;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (!(localATR > 0)) return false;

                // 1. Top 1: max(H[0]..H[4])
                double high1 = double.MinValue;
                int idx1 = 0;
                for (int i = 0; i <= 4; i++)
                {
                    double val = H(bars, endIndex, i);
                    if (val > high1) { high1 = val; idx1 = i; }
                }

                // 2. Top 2: max(H[7]..H[11]), vincolo idx2 <= 10
                double high2 = double.MinValue;
                int idx2 = 7;
                for (int i = 7; i <= 11; i++)
                {
                    double val = H(bars, endIndex, i);
                    if (val > high2) { high2 = val; idx2 = i; }
                }
                if (idx2 > 10) return false;

                // 3. Livelli simili: |high1 - high2| < 0.3 * localATR
                if (Math.Abs(high1 - high2) >= 0.3 * localATR) return false;

                // 4. Neckline: min(L[idx1]..L[idx2])
                double neckline = double.MaxValue;
                for (int i = idx1; i <= idx2; i++)
                {
                    neckline = Math.Min(neckline, L(bars, endIndex, i));
                }
                // neckline significativamente sotto i top
                if (neckline >= Math.Min(high1, high2) - 0.5 * localATR) return false;

                // 5. Break: C[11] < neckline
                if (C(bars, endIndex, 11) >= neckline) return false;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                // Caso A: ricalcola neckline e high direttamente
                double high1 = double.MinValue;
                int idx1 = 0;
                for (int i = 0; i <= 4; i++)
                {
                    double val = H(bars, endIndex, i);
                    if (val > high1) { high1 = val; idx1 = i; }
                }
                double high2 = double.MinValue;
                int idx2 = 7;
                for (int i = 7; i <= 11; i++)
                {
                    double val = H(bars, endIndex, i);
                    if (val > high2) { high2 = val; idx2 = i; }
                }
                double neckline = double.MaxValue;
                for (int i = idx1; i <= idx2; i++)
                {
                    neckline = Math.Min(neckline, L(bars, endIndex, i));
                }

                // Target bear: currentPrice + (neckline - max(high1, high2))
                // neckline < highs → negativo → target sotto currentPrice
                double target = currentPrice + (neckline - Math.Max(high1, high2));
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.3 Falling Wedge (Bull) ----
        private class FallingWedgePattern : PatternBase
        {
            public FallingWedgePattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Falling Wedge";
            public override int RequiredCandles => 10;
            public override int GhostCandleCount => 5;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (!(localATR > 0)) return false;

                // 1. Highs declinanti: almeno 3 coppie (i, i+2) dove H[i+2] < H[i], i pari in {0,2,4,6}
                int decliningHighs = 0;
                for (int i = 0; i <= 6; i += 2)
                {
                    if (H(bars, endIndex, i + 2) < H(bars, endIndex, i))
                        decliningHighs++;
                }
                if (decliningHighs < 3) return false;

                // 2. Lows declinanti: almeno 3 coppie (i, i+2) dove L[i+2] < L[i], i pari in {0,2,4,6}
                int decliningLows = 0;
                for (int i = 0; i <= 6; i += 2)
                {
                    if (L(bars, endIndex, i + 2) < L(bars, endIndex, i))
                        decliningLows++;
                }
                if (decliningLows < 3) return false;

                // 3. Convergenza del wedge: range[0] > range[8] * 1.3 AND range[4] > range[8]
                if (Range(bars, endIndex, 0) <= Range(bars, endIndex, 8) * 1.3) return false;
                if (Range(bars, endIndex, 4) <= Range(bars, endIndex, 8)) return false;

                // 4. Pendenza differenziale: (H[0] - H[8]) > (L[0] - L[8]) * 1.2
                double highDrop = H(bars, endIndex, 0) - H(bars, endIndex, 8);
                double lowDrop = L(bars, endIndex, 0) - L(bars, endIndex, 8);
                if (highDrop <= lowDrop * 1.2) return false;

                // 5. Break up: C[9] > C[8] AND body[9] > 0.1 * localATR
                if (C(bars, endIndex, 9) <= C(bars, endIndex, 8)) return false;
                if (Body(bars, endIndex, 9) <= 0.1 * localATR) return false;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                // Target bull: currentPrice + (H[0] - L[0]) — Caso A
                double target = currentPrice + (H(bars, endIndex, 0) - L(bars, endIndex, 0));
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.3 Rising Wedge (Bear) ----
        private class RisingWedgePattern : PatternBase
        {
            public RisingWedgePattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Rising Wedge";
            public override int RequiredCandles => 10;
            public override int GhostCandleCount => 5;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (!(localATR > 0)) return false;

                // 1. Highs crescenti: almeno 3 coppie (i, i+2) dove H[i+2] > H[i], i pari in {0,2,4,6}
                int risingHighs = 0;
                for (int i = 0; i <= 6; i += 2)
                {
                    if (H(bars, endIndex, i + 2) > H(bars, endIndex, i))
                        risingHighs++;
                }
                if (risingHighs < 3) return false;

                // 2. Lows crescenti: almeno 3 coppie (i, i+2) dove L[i+2] > L[i], i pari in {0,2,4,6}
                int risingLows = 0;
                for (int i = 0; i <= 6; i += 2)
                {
                    if (L(bars, endIndex, i + 2) > L(bars, endIndex, i))
                        risingLows++;
                }
                if (risingLows < 3) return false;

                // 3. Convergenza del wedge: range[0] > range[8] * 1.3 AND range[4] > range[8]
                if (Range(bars, endIndex, 0) <= Range(bars, endIndex, 8) * 1.3) return false;
                if (Range(bars, endIndex, 4) <= Range(bars, endIndex, 8)) return false;

                // 4. Pendenza differenziale bear: (L[8] - L[0]) > (H[8] - H[0]) * 1.2
                // I lows salgono piu' dei highs — convergenza verso l'alto
                double lowRise = L(bars, endIndex, 8) - L(bars, endIndex, 0);
                double highRise = H(bars, endIndex, 8) - H(bars, endIndex, 0);
                if (lowRise <= highRise * 1.2) return false;

                // 5. Break down: C[9] < C[8] AND body[9] > 0.1 * localATR
                if (C(bars, endIndex, 9) >= C(bars, endIndex, 8)) return false;
                if (Body(bars, endIndex, 9) <= 0.1 * localATR) return false;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                // Target bear: currentPrice - (H[0] - L[0]) — esplicito, NON mirror
                double target = currentPrice - (H(bars, endIndex, 0) - L(bars, endIndex, 0));
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.4 Asc Triangle (Bull) ----
        private class AscTrianglePattern : PatternBase
        {
            public AscTrianglePattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Asc Triangle";
            public override int RequiredCandles => 10;
            public override int GhostCandleCount => 5;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (!(localATR > 0)) return false;

                // 1. Resistenza piatta: maxHigh = max(H[0]..H[8])
                double maxHigh = double.MinValue;
                for (int i = 0; i <= 8; i++)
                    maxHigh = Math.Max(maxHigh, H(bars, endIndex, i));

                // Almeno 2 candele con high entro 0.2 * localATR da maxHigh
                int flatCount = 0;
                double flatSum = 0;
                for (int i = 0; i <= 8; i++)
                {
                    if (H(bars, endIndex, i) >= maxHigh - 0.2 * localATR)
                    {
                        flatCount++;
                        flatSum += H(bars, endIndex, i);
                    }
                }
                if (flatCount < 2) return false;
                double flatHigh = flatSum / flatCount;

                // 2. Lows crescenti: almeno 3 coppie (i, i+2) con i in {0,2,4,6} dove L[i+2] > L[i]
                int risingLows = 0;
                for (int i = 0; i <= 6; i += 2)
                {
                    if (L(bars, endIndex, i + 2) > L(bars, endIndex, i))
                        risingLows++;
                }
                if (risingLows < 3) return false;

                // 3. Convergenza: flatHigh - min(L[6],L[7],L[8]) < (flatHigh - min(L[0],L[1],L[2])) * 0.8
                double minLowEnd = Math.Min(Math.Min(L(bars, endIndex, 6), L(bars, endIndex, 7)), L(bars, endIndex, 8));
                double minLowStart = Math.Min(Math.Min(L(bars, endIndex, 0), L(bars, endIndex, 1)), L(bars, endIndex, 2));
                double distEnd = flatHigh - minLowEnd;
                double distStart = flatHigh - minLowStart;
                if (distStart <= 0) return false;
                if (distEnd >= distStart * 0.8) return false;

                // 4. Break: C[9] > flatHigh
                if (C(bars, endIndex, 9) <= flatHigh) return false;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                double localATR = LocalATR(endIndex);

                // Ricalcola flatHigh e minLow
                double maxHigh = double.MinValue;
                for (int i = 0; i <= 8; i++)
                    maxHigh = Math.Max(maxHigh, H(bars, endIndex, i));

                double flatSum = 0;
                int flatCount = 0;
                for (int i = 0; i <= 8; i++)
                {
                    if (H(bars, endIndex, i) >= maxHigh - 0.2 * localATR)
                    {
                        flatCount++;
                        flatSum += H(bars, endIndex, i);
                    }
                }
                double flatHigh = flatCount > 0 ? flatSum / flatCount : maxHigh;

                double minLow = double.MaxValue;
                for (int i = 0; i <= 8; i++)
                    minLow = Math.Min(minLow, L(bars, endIndex, i));

                // triangleHeight = flatHigh - min(L[0]..L[8]), cappato a 3 * localATR
                double triangleHeight = flatHigh - minLow;
                double cappedHeight = Math.Min(triangleHeight, 3 * localATR);
                double target = currentPrice + cappedHeight;
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.4 Desc Triangle (Bear) ----
        private class DescTrianglePattern : PatternBase
        {
            public DescTrianglePattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Desc Triangle";
            public override int RequiredCandles => 10;
            public override int GhostCandleCount => 5;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (!(localATR > 0)) return false;

                // 1. Supporto piatto: minLow = min(L[0]..L[8])
                double minLow = double.MaxValue;
                for (int i = 0; i <= 8; i++)
                    minLow = Math.Min(minLow, L(bars, endIndex, i));

                // Almeno 2 candele con low entro 0.2 * localATR da minLow
                int flatCount = 0;
                double flatSum = 0;
                for (int i = 0; i <= 8; i++)
                {
                    if (L(bars, endIndex, i) <= minLow + 0.2 * localATR)
                    {
                        flatCount++;
                        flatSum += L(bars, endIndex, i);
                    }
                }
                if (flatCount < 2) return false;
                double flatLow = flatSum / flatCount;

                // 2. Highs declinanti: almeno 3 coppie (i, i+2) con i in {0,2,4,6} dove H[i+2] < H[i]
                int decliningHighs = 0;
                for (int i = 0; i <= 6; i += 2)
                {
                    if (H(bars, endIndex, i + 2) < H(bars, endIndex, i))
                        decliningHighs++;
                }
                if (decliningHighs < 3) return false;

                // 3. Convergenza: max(H[6],H[7],H[8]) - flatLow < (max(H[0],H[1],H[2]) - flatLow) * 0.8
                double maxHighEnd = Math.Max(Math.Max(H(bars, endIndex, 6), H(bars, endIndex, 7)), H(bars, endIndex, 8));
                double maxHighStart = Math.Max(Math.Max(H(bars, endIndex, 0), H(bars, endIndex, 1)), H(bars, endIndex, 2));
                double distEnd = maxHighEnd - flatLow;
                double distStart = maxHighStart - flatLow;
                if (distStart <= 0) return false;
                if (distEnd >= distStart * 0.8) return false;

                // 4. Break: C[9] < flatLow
                if (C(bars, endIndex, 9) >= flatLow) return false;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                double localATR = LocalATR(endIndex);

                // Ricalcola flatLow e maxHigh
                double minLow = double.MaxValue;
                for (int i = 0; i <= 8; i++)
                    minLow = Math.Min(minLow, L(bars, endIndex, i));

                double flatSum = 0;
                int flatCount = 0;
                for (int i = 0; i <= 8; i++)
                {
                    if (L(bars, endIndex, i) <= minLow + 0.2 * localATR)
                    {
                        flatCount++;
                        flatSum += L(bars, endIndex, i);
                    }
                }
                double flatLow = flatCount > 0 ? flatSum / flatCount : minLow;

                double maxHigh = double.MinValue;
                for (int i = 0; i <= 8; i++)
                    maxHigh = Math.Max(maxHigh, H(bars, endIndex, i));

                // triangleHeight = max(H[0]..H[8]) - flatLow, cappato a 3 * localATR
                double triangleHeight = maxHigh - flatLow;
                double cappedHeight = Math.Min(triangleHeight, 3 * localATR);
                double target = currentPrice - cappedHeight;
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.5 Rising Three Methods (Bull) ----
        private class RisingThreeMethodsPattern : PatternBase
        {
            public RisingThreeMethodsPattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Rising Three Methods";
            public override int RequiredCandles => 5;
            public override int GhostCandleCount => 4;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (!(localATR > 0)) return false;

                // 1. Prima candela forte bullish con corpo solido
                if (!IsBull(bars, endIndex, 0)) return false;
                if (Body(bars, endIndex, 0) <= 0.6 * Range(bars, endIndex, 0)) return false;

                // 2. Tre candele contenute (c[1]-c[3]) dentro il range di c[0], almeno 2/3 bearish
                int bearCount = 0;
                for (int i = 1; i <= 3; i++)
                {
                    if (H(bars, endIndex, i) > H(bars, endIndex, 0)) return false;
                    if (L(bars, endIndex, i) < L(bars, endIndex, 0)) return false;
                    if (IsBear(bars, endIndex, i)) bearCount++;
                }
                if (bearCount < 2) return false;

                // 3. Ultima candela rompe sopra: bull, C[4] > H[0], corpo significativo
                if (!IsBull(bars, endIndex, 4)) return false;
                if (C(bars, endIndex, 4) <= H(bars, endIndex, 0)) return false;
                if (Body(bars, endIndex, 4) <= 0.5 * Range(bars, endIndex, 4)) return false;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                // Target: currentPrice + (C[0] - O[0]) + (C[4] - O[4]) — Caso A
                // Bull: entrambe bullish → positivo → target sopra currentPrice
                double target = currentPrice
                    + (C(bars, endIndex, 0) - O(bars, endIndex, 0))
                    + (C(bars, endIndex, 4) - O(bars, endIndex, 4));
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.5 Falling Three Methods (Bear — Mirror) ----
        private class FallingThreeMethodsPattern : PatternBase
        {
            public FallingThreeMethodsPattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Falling Three Methods";
            public override int RequiredCandles => 5;
            public override int GhostCandleCount => 4;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (!(localATR > 0)) return false;

                // 1. Prima candela forte bearish con corpo solido
                if (!IsBear(bars, endIndex, 0)) return false;
                if (Body(bars, endIndex, 0) <= 0.6 * Range(bars, endIndex, 0)) return false;

                // 2. Tre candele contenute (c[1]-c[3]) dentro il range di c[0], almeno 2/3 bullish
                int bullCount = 0;
                for (int i = 1; i <= 3; i++)
                {
                    if (H(bars, endIndex, i) > H(bars, endIndex, 0)) return false;
                    if (L(bars, endIndex, i) < L(bars, endIndex, 0)) return false;
                    if (IsBull(bars, endIndex, i)) bullCount++;
                }
                if (bullCount < 2) return false;

                // 3. Ultima candela rompe sotto: bear, C[4] < L[0], corpo significativo
                if (!IsBear(bars, endIndex, 4)) return false;
                if (C(bars, endIndex, 4) >= L(bars, endIndex, 0)) return false;
                if (Body(bars, endIndex, 4) <= 0.5 * Range(bars, endIndex, 4)) return false;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                // Target: currentPrice + (C[0] - O[0]) + (C[4] - O[4]) — formula IDENTICA
                // Bear: entrambe bearish → C<O → negativo → target sotto currentPrice
                double target = currentPrice
                    + (C(bars, endIndex, 0) - O(bars, endIndex, 0))
                    + (C(bars, endIndex, 4) - O(bars, endIndex, 4));
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.6 Bull Three Line Strike ----
        private class BullThreeLineStrikePattern : PatternBase
        {
            public BullThreeLineStrikePattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bull Three Line Strike";
            public override int RequiredCandles => 4;
            public override int GhostCandleCount => 5;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. Tre candele bullish consecutive con closes crescenti
                if (!IsBull(bars, endIndex, 0)) return false;
                if (!IsBull(bars, endIndex, 1)) return false;
                if (!IsBull(bars, endIndex, 2)) return false;
                if (C(bars, endIndex, 1) <= C(bars, endIndex, 0)) return false;
                if (C(bars, endIndex, 2) <= C(bars, endIndex, 1)) return false;

                // 2. Quarta candela engulfing bearish: apre sopra C[2], chiude sotto O[0]
                if (!IsBear(bars, endIndex, 3)) return false;
                if (O(bars, endIndex, 3) <= C(bars, endIndex, 2)) return false;
                if (C(bars, endIndex, 3) >= O(bars, endIndex, 0)) return false;

                // 3. Corpo dominante: body[3] > body[0] + body[1] + body[2]
                double sumBodies = Body(bars, endIndex, 0) + Body(bars, endIndex, 1) + Body(bars, endIndex, 2);
                if (Body(bars, endIndex, 3) <= sumBodies) return false;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                // Target: currentPrice + (C[2] - O[0]) — Caso A
                // Bull: C[2] > O[0] → positivo → target sopra currentPrice
                double target = currentPrice + (C(bars, endIndex, 2) - O(bars, endIndex, 0));
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.6 Bear Three Line Strike (Mirror) ----
        private class BearThreeLineStrikePattern : PatternBase
        {
            public BearThreeLineStrikePattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bear Three Line Strike";
            public override int RequiredCandles => 4;
            public override int GhostCandleCount => 5;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. Tre candele bearish consecutive con closes decrescenti
                if (!IsBear(bars, endIndex, 0)) return false;
                if (!IsBear(bars, endIndex, 1)) return false;
                if (!IsBear(bars, endIndex, 2)) return false;
                if (C(bars, endIndex, 1) >= C(bars, endIndex, 0)) return false;
                if (C(bars, endIndex, 2) >= C(bars, endIndex, 1)) return false;

                // 2. Quarta candela engulfing bullish: apre sotto C[2], chiude sopra O[0]
                if (!IsBull(bars, endIndex, 3)) return false;
                if (O(bars, endIndex, 3) >= C(bars, endIndex, 2)) return false;
                if (C(bars, endIndex, 3) <= O(bars, endIndex, 0)) return false;

                // 3. Corpo dominante: body[3] > body[0] + body[1] + body[2]
                double sumBodies = Body(bars, endIndex, 0) + Body(bars, endIndex, 1) + Body(bars, endIndex, 2);
                if (Body(bars, endIndex, 3) <= sumBodies) return false;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                // Target: currentPrice + (C[2] - O[0]) — formula IDENTICA
                // Bear: C[2] < O[0] (tre closes bearish sotto primo open) → negativo → target sotto currentPrice
                double target = currentPrice + (C(bars, endIndex, 2) - O(bars, endIndex, 0));
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.7 Bull Hikkake ----
        private class BullHikkakePattern : PatternBase
        {
            public BullHikkakePattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bull Hikkake";
            public override int RequiredCandles => 6;
            public override int GhostCandleCount => 5;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. Mother bar significativa: range[0] > 0.3 * localATR
                double motherRange = H(bars, endIndex, 0) - L(bars, endIndex, 0);
                if (motherRange <= 0.3 * localATR) return false;

                // 2. Inside bar: H[1] < H[0] E L[1] > L[0]
                if (H(bars, endIndex, 1) >= H(bars, endIndex, 0)) return false;
                if (L(bars, endIndex, 1) <= L(bars, endIndex, 0)) return false;

                // 3. Fake breakout down: L[2] < L[1]
                if (L(bars, endIndex, 2) >= L(bars, endIndex, 1)) return false;

                // 4. Rientro: almeno una candela tra c[3] e c[4] con C[i] > H[1]
                bool rientro = false;
                for (int i = 3; i <= 4; i++)
                {
                    if (C(bars, endIndex, i) > H(bars, endIndex, 1))
                    { rientro = true; break; }
                }
                if (!rientro) return false;

                // 5. Break finale: C[5] > H[0]
                if (C(bars, endIndex, 5) <= H(bars, endIndex, 0)) return false;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                // Target: cappedRange = min(motherRange, 1.5 * localATR)
                // motherRange = H[0] - L[0] (sempre positivo)
                double motherRange = H(bars, endIndex, 0) - L(bars, endIndex, 0);
                double localATR = LocalATR(endIndex);
                double cappedRange = Math.Min(motherRange, 1.5 * localATR);
                double target = currentPrice + cappedRange;
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.7 Bear Hikkake (mirror) ----
        private class BearHikkakePattern : PatternBase
        {
            public BearHikkakePattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bear Hikkake";
            public override int RequiredCandles => 6;
            public override int GhostCandleCount => 5;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. Mother bar significativa: range[0] > 0.3 * localATR
                // NON mirror: range e' sempre H-L positivo
                double motherRange = H(bars, endIndex, 0) - L(bars, endIndex, 0);
                if (motherRange <= 0.3 * localATR) return false;

                // 2. Inside bar: H[1] < H[0] E L[1] > L[0] (identico al bull)
                if (H(bars, endIndex, 1) >= H(bars, endIndex, 0)) return false;
                if (L(bars, endIndex, 1) <= L(bars, endIndex, 0)) return false;

                // 3. Fake breakout UP: H[2] > H[1] (mirror di L[2] < L[1])
                if (H(bars, endIndex, 2) <= H(bars, endIndex, 1)) return false;

                // 4. Rientro sotto: almeno una candela tra c[3] e c[4] con C[i] < L[1]
                bool rientro = false;
                for (int i = 3; i <= 4; i++)
                {
                    if (C(bars, endIndex, i) < L(bars, endIndex, 1))
                    { rientro = true; break; }
                }
                if (!rientro) return false;

                // 5. Break finale down: C[5] < L[0] (mirror di C[5] > H[0])
                if (C(bars, endIndex, 5) >= L(bars, endIndex, 0)) return false;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                // Target bear: currentPrice - cappedRange
                // motherRange = H[0] - L[0] (sempre positivo, NON mirror)
                double motherRange = H(bars, endIndex, 0) - L(bars, endIndex, 0);
                double localATR = LocalATR(endIndex);
                double cappedRange = Math.Min(motherRange, 1.5 * localATR);
                double target = currentPrice - cappedRange;
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.8 Bull Channel Breakout (bear esplicito) ----
        private class BullChannelBreakoutPattern : PatternBase
        {
            public BullChannelBreakoutPattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bull Channel Breakout";
            public override int RequiredCandles => 10;
            public override int GhostCandleCount => 5;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. Canale (c[0]-c[8]): channelHigh, channelLow, channelRange
                double channelHigh = double.MinValue;
                double channelLow = double.MaxValue;
                for (int i = 0; i <= 8; i++)
                {
                    channelHigh = Math.Max(channelHigh, H(bars, endIndex, i));
                    channelLow = Math.Min(channelLow, L(bars, endIndex, i));
                }
                double channelRange = channelHigh - channelLow;

                // 2. Lateralita': channelRange < 2.0 * localATR
                if (channelRange >= 2.0 * localATR) return false;

                // Guard: canale degenerato — soglie di tocco collasserebbero
                if (channelRange < 0.1 * localATR) return false;

                // 3. Almeno 2 tocchi top: H[i] > channelHigh - 0.15 * channelRange
                double topThreshold = channelHigh - 0.15 * channelRange;
                int topTouches = 0;
                for (int i = 0; i <= 8; i++)
                {
                    if (H(bars, endIndex, i) > topThreshold) topTouches++;
                }
                if (topTouches < 2) return false;

                // 4. Almeno 2 tocchi bottom: L[i] < channelLow + 0.15 * channelRange
                double bottomThreshold = channelLow + 0.15 * channelRange;
                int bottomTouches = 0;
                for (int i = 0; i <= 8; i++)
                {
                    if (L(bars, endIndex, i) < bottomThreshold) bottomTouches++;
                }
                if (bottomTouches < 2) return false;

                // 5. Oscillazione vera: tocchi top e bottom distribuiti nel tempo
                bool topFirst5 = false, bottomFirst5 = false;
                bool topLast5 = false, bottomLast5 = false;
                for (int i = 0; i <= 4; i++)
                {
                    if (H(bars, endIndex, i) > topThreshold) topFirst5 = true;
                    if (L(bars, endIndex, i) < bottomThreshold) bottomFirst5 = true;
                }
                for (int i = 4; i <= 8; i++)
                {
                    if (H(bars, endIndex, i) > topThreshold) topLast5 = true;
                    if (L(bars, endIndex, i) < bottomThreshold) bottomLast5 = true;
                }
                // topFirst5 && bottomLast5 OPPURE bottomFirst5 && topLast5
                bool oscillation = (topFirst5 && bottomLast5) || (bottomFirst5 && topLast5);
                if (!oscillation) return false;

                // 6. Break: C[9] > channelHigh
                if (C(bars, endIndex, 9) <= channelHigh) return false;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                // Target: currentPrice + channelRange
                double channelHigh = double.MinValue;
                double channelLow = double.MaxValue;
                for (int i = 0; i <= 8; i++)
                {
                    channelHigh = Math.Max(channelHigh, H(bars, endIndex, i));
                    channelLow = Math.Min(channelLow, L(bars, endIndex, i));
                }
                double channelRange = channelHigh - channelLow;
                double target = currentPrice + channelRange;
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.8 Bear Channel Breakout (bear esplicito — solo break diverso) ----
        private class BearChannelBreakoutPattern : PatternBase
        {
            public BearChannelBreakoutPattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bear Channel Breakout";
            public override int RequiredCandles => 10;
            public override int GhostCandleCount => 5;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. Canale (c[0]-c[8]): identico al bull
                double channelHigh = double.MinValue;
                double channelLow = double.MaxValue;
                for (int i = 0; i <= 8; i++)
                {
                    channelHigh = Math.Max(channelHigh, H(bars, endIndex, i));
                    channelLow = Math.Min(channelLow, L(bars, endIndex, i));
                }
                double channelRange = channelHigh - channelLow;

                // 2. Lateralita': channelRange < 2.0 * localATR
                if (channelRange >= 2.0 * localATR) return false;

                // Guard: canale degenerato — soglie di tocco collasserebbero
                if (channelRange < 0.1 * localATR) return false;

                // 3. Almeno 2 tocchi top
                double topThreshold = channelHigh - 0.15 * channelRange;
                int topTouches = 0;
                for (int i = 0; i <= 8; i++)
                {
                    if (H(bars, endIndex, i) > topThreshold) topTouches++;
                }
                if (topTouches < 2) return false;

                // 4. Almeno 2 tocchi bottom
                double bottomThreshold = channelLow + 0.15 * channelRange;
                int bottomTouches = 0;
                for (int i = 0; i <= 8; i++)
                {
                    if (L(bars, endIndex, i) < bottomThreshold) bottomTouches++;
                }
                if (bottomTouches < 2) return false;

                // 5. Oscillazione vera (identica al bull)
                bool topFirst5 = false, bottomFirst5 = false;
                bool topLast5 = false, bottomLast5 = false;
                for (int i = 0; i <= 4; i++)
                {
                    if (H(bars, endIndex, i) > topThreshold) topFirst5 = true;
                    if (L(bars, endIndex, i) < bottomThreshold) bottomFirst5 = true;
                }
                for (int i = 4; i <= 8; i++)
                {
                    if (H(bars, endIndex, i) > topThreshold) topLast5 = true;
                    if (L(bars, endIndex, i) < bottomThreshold) bottomLast5 = true;
                }
                bool oscillation = (topFirst5 && bottomLast5) || (bottomFirst5 && topLast5);
                if (!oscillation) return false;

                // 6. Break DOWN: C[9] < channelLow (unica differenza dal bull)
                if (C(bars, endIndex, 9) >= channelLow) return false;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                // Target: currentPrice - channelRange (negativo per bear)
                // channelRange = channelHigh - channelLow (sempre positivo)
                double channelHigh = double.MinValue;
                double channelLow = double.MaxValue;
                for (int i = 0; i <= 8; i++)
                {
                    channelHigh = Math.Max(channelHigh, H(bars, endIndex, i));
                    channelLow = Math.Min(channelLow, L(bars, endIndex, i));
                }
                double channelRange = channelHigh - channelLow;
                double target = currentPrice - channelRange;
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.9 Bull Quasimodo ----
        private class BullQuasimodoPattern : PatternBase
        {
            private double _sH2Price;
            private double _sL2Price;

            public BullQuasimodoPattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bull Quasimodo";
            public override int RequiredCandles => 12;
            public override int GhostCandleCount => 5;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. FindSwings — need sH1, sL1, sH2, sL2 (first must be high)
                var swings = FindSwings(bars, endIndex - 11, endIndex);
                int offset = 0;
                if (swings.Count > 0 && !swings[0].IsHigh)
                    offset = 1;
                if (swings.Count < 4 + offset) return false;

                var sH1 = swings[offset];
                var sL1 = swings[offset + 1];
                var sH2 = swings[offset + 2];
                var sL2 = swings[offset + 3];

                // Verify alternation: H, L, H, L
                if (!sH1.IsHigh || sL1.IsHigh || !sH2.IsHigh || sL2.IsHigh) return false;

                // sL2 must leave at least 2 candles of reaction
                if (sL2.Index > endIndex - 2) return false;

                // 2. Higher high
                if (sH2.Price <= sH1.Price) return false;

                // 3. Lower low (asymmetric shoulder)
                if (sL2.Price >= sL1.Price) return false;

                // 4. Minimum amplitude
                if (sH2.Price - sL2.Price <= 0.5 * localATR) return false;

                // 5. Reaction: price rises back above first swing low
                if (C(bars, endIndex, 11) <= sL1.Price) return false;

                _sH2Price = sH2.Price;
                _sL2Price = sL2.Price;
                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                // Caso B: refresh private fields
                if (!CheckPattern(bars, endIndex))
                    return new List<GhostCandle>();
                double target = currentPrice + (_sH2Price - _sL2Price) * 0.5;
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.9 Bear Quasimodo ----
        private class BearQuasimodoPattern : PatternBase
        {
            private double _sL2Price;
            private double _sH2Price;

            public BearQuasimodoPattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bear Quasimodo";
            public override int RequiredCandles => 12;
            public override int GhostCandleCount => 5;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. FindSwings — need sL1, sH1, sL2, sH2 (first must be low)
                var swings = FindSwings(bars, endIndex - 11, endIndex);
                int offset = 0;
                if (swings.Count > 0 && swings[0].IsHigh)
                    offset = 1;
                if (swings.Count < 4 + offset) return false;

                var sL1 = swings[offset];
                var sH1 = swings[offset + 1];
                var sL2 = swings[offset + 2];
                var sH2 = swings[offset + 3];

                // Verify alternation: L, H, L, H
                if (sL1.IsHigh || !sH1.IsHigh || sL2.IsHigh || !sH2.IsHigh) return false;

                // sH2 must leave at least 2 candles of reaction
                if (sH2.Index > endIndex - 2) return false;

                // 2. Lower low
                if (sL2.Price >= sL1.Price) return false;

                // 3. Higher high
                if (sH2.Price <= sH1.Price) return false;

                // 4. Minimum amplitude (REAL names, not mirror)
                if (sH2.Price - sL2.Price <= 0.5 * localATR) return false;

                // 5. Reaction: price drops below first swing high
                if (C(bars, endIndex, 11) >= sH1.Price) return false;

                _sL2Price = sL2.Price;
                _sH2Price = sH2.Price;
                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                // Caso B: refresh private fields
                if (!CheckPattern(bars, endIndex))
                    return new List<GhostCandle>();
                // Mirror target: sL2(real low) - sH2(real high) → negative
                double target = currentPrice + (_sL2Price - _sH2Price) * 0.5;
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.10 Bull Spike Channel ----
        private class BullSpikeChannelPattern : PatternBase
        {
            public BullSpikeChannelPattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bull Spike Channel";
            public override int RequiredCandles => 12;
            public override int GhostCandleCount => 5;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. Spike (c[0]-c[2]): strong bullish impulse
                double spikeSize = C(bars, endIndex, 2) - O(bars, endIndex, 0);
                if (spikeSize <= 2.0 * localATR) return false;

                // At least 2 of 3 candles bullish
                int bullCount = 0;
                for (int i = 0; i <= 2; i++)
                    if (IsBull(bars, endIndex, i)) bullCount++;
                if (bullCount < 2) return false;

                // 2. Channel down — declining highs (c[3]-c[10])
                // At least 3 pairs (i, i+2) with H[i+2] < H[i], i in {3..8}
                int decliningHighs = 0;
                for (int i = 3; i <= 8; i++)
                    if (H(bars, endIndex, i + 2) < H(bars, endIndex, i)) decliningHighs++;
                if (decliningHighs < 3) return false;

                // 3. Channel down — declining lows (c[3]-c[10])
                // At least 2 pairs (i, i+2) with L[i+2] < L[i], i in {3..8}
                int decliningLows = 0;
                for (int i = 3; i <= 8; i++)
                    if (L(bars, endIndex, i + 2) < L(bars, endIndex, i)) decliningLows++;
                if (decliningLows < 2) return false;

                // 4. Channel depth: H[3] - H[10] > 0.3 * localATR
                if (H(bars, endIndex, 3) - H(bars, endIndex, 10) <= 0.3 * localATR) return false;

                // 5. Retracement limited: min(L[3]..L[10]) > O[0] + spike * 0.3
                double minLow = double.MaxValue;
                for (int i = 3; i <= 10; i++)
                    minLow = Math.Min(minLow, L(bars, endIndex, i));
                double retracementLevel = O(bars, endIndex, 0) + spikeSize * 0.3;
                if (minLow <= retracementLevel) return false;

                // 6. Break: C[11] > H[10]
                if (C(bars, endIndex, 11) <= H(bars, endIndex, 10)) return false;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                // Caso A: target directly from candle data
                double target = currentPrice + (C(bars, endIndex, 2) - O(bars, endIndex, 0)) * 0.618;
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.10 Bear Spike Channel ----
        private class BearSpikeChannelPattern : PatternBase
        {
            public BearSpikeChannelPattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bear Spike Channel";
            public override int RequiredCandles => 12;
            public override int GhostCandleCount => 5;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. Spike bear: O[0] - C[2] > 2.0 * localATR (strong bearish impulse)
                double spikeSize = O(bars, endIndex, 0) - C(bars, endIndex, 2);
                if (spikeSize <= 2.0 * localATR) return false;

                // At least 2 of 3 candles bearish
                int bearCount = 0;
                for (int i = 0; i <= 2; i++)
                    if (IsBear(bars, endIndex, i)) bearCount++;
                if (bearCount < 2) return false;

                // 2. Channel up — rising highs (c[3]-c[10])
                // At least 3 pairs (i, i+2) with H[i+2] > H[i], i in {3..8}
                int risingHighs = 0;
                for (int i = 3; i <= 8; i++)
                    if (H(bars, endIndex, i + 2) > H(bars, endIndex, i)) risingHighs++;
                if (risingHighs < 3) return false;

                // 3. Channel up — rising lows (c[3]-c[10])
                // At least 2 pairs (i, i+2) with L[i+2] > L[i], i in {3..8}
                int risingLows = 0;
                for (int i = 3; i <= 8; i++)
                    if (L(bars, endIndex, i + 2) > L(bars, endIndex, i)) risingLows++;
                if (risingLows < 2) return false;

                // 4. Channel depth: L[10] - L[3] > 0.3 * localATR
                if (L(bars, endIndex, 10) - L(bars, endIndex, 3) <= 0.3 * localATR) return false;

                // 5. Retracement limited: max(H[3]..H[10]) < O[0] - spike * 0.3
                double maxHigh = double.MinValue;
                for (int i = 3; i <= 10; i++)
                    maxHigh = Math.Max(maxHigh, H(bars, endIndex, i));
                double retracementLevel = O(bars, endIndex, 0) - spikeSize * 0.3;
                if (maxHigh >= retracementLevel) return false;

                // 6. Break: C[11] < L[10]
                if (C(bars, endIndex, 11) >= L(bars, endIndex, 10)) return false;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                // Target: same formula as bull — C[2] < O[0] in bear → negative
                double target = currentPrice + (C(bars, endIndex, 2) - O(bars, endIndex, 0)) * 0.618;
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.11 Bull ABCD ----
        private class BullABCDPattern : PatternBase
        {
            private double _aPrice;
            private double _bPrice;

            public BullABCDPattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bull ABCD";
            public override int RequiredCandles => 16;
            public override int GhostCandleCount => 5;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. FindSwings — need A(high), B(low), C(high), D(low)
                var swings = FindSwings(bars, endIndex - 15, endIndex);
                int offset = 0;
                if (swings.Count > 0 && !swings[0].IsHigh)
                    offset = 1;
                if (swings.Count < 4 + offset) return false;

                var A = swings[offset];
                var B = swings[offset + 1];
                var C = swings[offset + 2];
                var D = swings[offset + 3];

                if (!A.IsHigh || B.IsHigh || !C.IsHigh || D.IsHigh) return false;

                // 2. C below A
                if (C.Price >= A.Price) return false;

                // 3. AB significant
                double AB = A.Price - B.Price;
                if (AB <= 1.0 * localATR) return false;

                // 4. Retracement BC Fibonacci
                double BC = C.Price - B.Price;
                if (!FibCheck(BC / AB, 0.618, 0.1)) return false;

                // 5. Symmetry CD ≈ AB
                double CD = C.Price - D.Price;
                if (Math.Abs(CD - AB) >= 0.3 * Math.Abs(AB)) return false;

                // 6. D recent (last 4 bars)
                if (D.Index < endIndex - 3) return false;

                _aPrice = A.Price;
                _bPrice = B.Price;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                if (!CheckPattern(bars, endIndex))
                    return new List<GhostCandle>();
                double AB = _aPrice - _bPrice;
                double target = currentPrice + AB * 0.618;
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.11 Bear ABCD ----
        private class BearABCDPattern : PatternBase
        {
            private double _aPrice;
            private double _bPrice;

            public BearABCDPattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bear ABCD";
            public override int RequiredCandles => 16;
            public override int GhostCandleCount => 5;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. FindSwings — need A(low), B(high), C(low), D(high)
                var swings = FindSwings(bars, endIndex - 15, endIndex);
                int offset = 0;
                if (swings.Count > 0 && swings[0].IsHigh)
                    offset = 1;
                if (swings.Count < 4 + offset) return false;

                var A = swings[offset];
                var B = swings[offset + 1];
                var C = swings[offset + 2];
                var D = swings[offset + 3];

                if (A.IsHigh || !B.IsHigh || C.IsHigh || !D.IsHigh) return false;

                // 2. C above A (retracement doesn't go below starting point)
                if (C.Price <= A.Price) return false;

                // 3. AB significant
                double AB = A.Price - B.Price; // negative (low - high)
                if (Math.Abs(AB) <= 1.0 * localATR) return false;

                // 4. Retracement BC Fibonacci
                double BC = C.Price - B.Price; // negative/negative = positive ratio
                if (!FibCheck(BC / AB, 0.618, 0.1)) return false;

                // 5. Symmetry CD ≈ AB
                double CD = C.Price - D.Price;
                if (Math.Abs(CD - AB) >= 0.3 * Math.Abs(AB)) return false;

                // 6. D recent (last 4 bars)
                if (D.Index < endIndex - 3) return false;

                _aPrice = A.Price;
                _bPrice = B.Price;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                if (!CheckPattern(bars, endIndex))
                    return new List<GhostCandle>();
                double AB = _aPrice - _bPrice; // negative for bear
                double target = currentPrice + AB * 0.618; // target < currentPrice
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.12 Bull Adam Eve ----
        private class BullAdamEvePattern : PatternBase
        {
            public BullAdamEvePattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bull Adam Eve";
            public override int RequiredCandles => 14;
            public override int GhostCandleCount => 5;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. Adam — bottom spike (c[0]-c[4])
                double adamLow = double.MaxValue;
                int adamIdx = 0;
                for (int i = 0; i <= 4; i++)
                {
                    double val = L(bars, endIndex, i);
                    if (val < adamLow) { adamLow = val; adamIdx = i; }
                }
                if (Range(bars, endIndex, adamIdx) <= 1.5 * localATR) return false;

                // 2. Eve — rounded bottom (c[8]-c[12])
                double eveLow = double.MaxValue;
                for (int i = 8; i <= 12; i++)
                    eveLow = Math.Min(eveLow, L(bars, endIndex, i));
                int eveCloseCount = 0;
                for (int i = 8; i <= 12; i++)
                    if (Math.Abs(L(bars, endIndex, i) - eveLow) <= 0.2 * localATR) eveCloseCount++;
                if (eveCloseCount < 3) return false;

                // 3. Similar levels
                if (Math.Abs(adamLow - eveLow) >= 0.4 * localATR) return false;

                // 4. Neckline
                double neckline = double.MinValue;
                for (int i = 4; i <= 8; i++)
                    neckline = Math.Max(neckline, H(bars, endIndex, i));

                // 5. Break above neckline
                if (C(bars, endIndex, 13) <= neckline) return false;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                double localATR = LocalATR(endIndex);

                double adamLow = double.MaxValue;
                for (int i = 0; i <= 4; i++)
                    adamLow = Math.Min(adamLow, L(bars, endIndex, i));

                double eveLow = double.MaxValue;
                for (int i = 8; i <= 12; i++)
                    eveLow = Math.Min(eveLow, L(bars, endIndex, i));

                double neckline = double.MinValue;
                for (int i = 4; i <= 8; i++)
                    neckline = Math.Max(neckline, H(bars, endIndex, i));

                double patternHeight = neckline - Math.Min(adamLow, eveLow);
                double cappedHeight = Math.Min(patternHeight, 2.0 * localATR);
                double target = currentPrice + cappedHeight;
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.12 Bear Adam Eve ----
        private class BearAdamEvePattern : PatternBase
        {
            public BearAdamEvePattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bear Adam Eve";
            public override int RequiredCandles => 14;
            public override int GhostCandleCount => 5;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. Adam — top spike (c[0]-c[4])
                double adamHigh = double.MinValue;
                int adamIdx = 0;
                for (int i = 0; i <= 4; i++)
                {
                    double val = H(bars, endIndex, i);
                    if (val > adamHigh) { adamHigh = val; adamIdx = i; }
                }
                if (Range(bars, endIndex, adamIdx) <= 1.5 * localATR) return false;

                // 2. Eve — rounded top (c[8]-c[12])
                double eveHigh = double.MinValue;
                for (int i = 8; i <= 12; i++)
                    eveHigh = Math.Max(eveHigh, H(bars, endIndex, i));
                int eveCloseCount = 0;
                for (int i = 8; i <= 12; i++)
                    if (Math.Abs(H(bars, endIndex, i) - eveHigh) <= 0.2 * localATR) eveCloseCount++;
                if (eveCloseCount < 3) return false;

                // 3. Similar levels
                if (Math.Abs(adamHigh - eveHigh) >= 0.4 * localATR) return false;

                // 4. Neckline (min L between the two tops)
                double neckline = double.MaxValue;
                for (int i = 4; i <= 8; i++)
                    neckline = Math.Min(neckline, L(bars, endIndex, i));

                // 5. Break below neckline
                if (C(bars, endIndex, 13) >= neckline) return false;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                double localATR = LocalATR(endIndex);

                double adamHigh = double.MinValue;
                for (int i = 0; i <= 4; i++)
                    adamHigh = Math.Max(adamHigh, H(bars, endIndex, i));

                double eveHigh = double.MinValue;
                for (int i = 8; i <= 12; i++)
                    eveHigh = Math.Max(eveHigh, H(bars, endIndex, i));

                double neckline = double.MaxValue;
                for (int i = 4; i <= 8; i++)
                    neckline = Math.Min(neckline, L(bars, endIndex, i));

                double patternHeight = Math.Max(adamHigh, eveHigh) - neckline;
                double cappedHeight = Math.Min(patternHeight, 2.0 * localATR);
                double target = currentPrice - cappedHeight;
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.13 Bull Liq Grab ----
        // (coppia: bear esplicito, non mirror meccanico)
        private class BullLiqGrabPattern : PatternBase
        {
            public BullLiqGrabPattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bull Liq Grab";
            public override int RequiredCandles => 5;
            public override int GhostCandleCount => 4;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. Support level: min(L[0], L[1], L[2])
                double support = Math.Min(L(bars, endIndex, 0),
                    Math.Min(L(bars, endIndex, 1), L(bars, endIndex, 2)));

                // 2. Grab (c[3])
                // L[3] < support (breaks below)
                if (L(bars, endIndex, 3) >= support) return false;
                // C[3] > support (closes above = rejection)
                if (C(bars, endIndex, 3) <= support) return false;
                // Grab significativo: support - L[3] > 0.15 * localATR
                if (support - L(bars, endIndex, 3) <= 0.15 * localATR) return false;
                // Wick inferiore prominente: min(O[3],C[3]) - L[3] > 0.5 * range[3]
                double range3 = Range(bars, endIndex, 3);
                if (range3 < 0.1 * localATR) return false;
                if (Math.Min(O(bars, endIndex, 3), C(bars, endIndex, 3))
                    - L(bars, endIndex, 3) <= 0.5 * range3) return false;

                // 3. Completamento (c[4]): bull[4] E C[4] > H[3]
                if (!IsBull(bars, endIndex, 4)) return false;
                if (C(bars, endIndex, 4) <= H(bars, endIndex, 3)) return false;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                double localATR = LocalATR(endIndex);
                double grabRange = (H(bars, endIndex, 3) - L(bars, endIndex, 3)) * 2;
                double cappedTarget = Math.Min(grabRange, 1.5 * localATR);
                double target = currentPrice + cappedTarget;
                return GenerateGhostsToTarget(
                    bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.13 Bear Liq Grab (bear esplicito) ----
        private class BearLiqGrabPattern : PatternBase
        {
            public BearLiqGrabPattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bear Liq Grab";
            public override int RequiredCandles => 5;
            public override int GhostCandleCount => 4;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. Resistance level: max(H[0], H[1], H[2])
                double resistance = Math.Max(H(bars, endIndex, 0),
                    Math.Max(H(bars, endIndex, 1), H(bars, endIndex, 2)));

                // 2. Grab (c[3])
                // H[3] > resistance (breaks above)
                if (H(bars, endIndex, 3) <= resistance) return false;
                // C[3] < resistance (closes below = rejection)
                if (C(bars, endIndex, 3) >= resistance) return false;
                // Grab significativo: H[3] - resistance > 0.15 * localATR
                if (H(bars, endIndex, 3) - resistance <= 0.15 * localATR) return false;
                // Wick superiore prominente: H[3] - max(O[3],C[3]) > 0.5 * range[3]
                double range3 = Range(bars, endIndex, 3);
                if (range3 < 0.1 * localATR) return false;
                if (H(bars, endIndex, 3)
                    - Math.Max(O(bars, endIndex, 3), C(bars, endIndex, 3))
                    <= 0.5 * range3) return false;

                // 3. Completamento (c[4]): bear[4] E C[4] < L[3]
                if (!IsBear(bars, endIndex, 4)) return false;
                if (C(bars, endIndex, 4) >= L(bars, endIndex, 3)) return false;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                double localATR = LocalATR(endIndex);
                // (L[3] - H[3]) * 2 e' negativo
                double grabRange = (L(bars, endIndex, 3) - H(bars, endIndex, 3)) * 2;
                // max per clampare il valore negativo a -1.5 * localATR
                double cappedTarget = Math.Max(grabRange, -1.5 * localATR);
                double target = currentPrice + cappedTarget;
                return GenerateGhostsToTarget(
                    bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.14 Inverse H&S (Bull) ----
        private class InverseHSPattern : PatternBase
        {
            private double _necklineAtEnd;
            private double _sL2Price;

            public InverseHSPattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Inverse H&S";
            public override int RequiredCandles => 18;
            public override int GhostCandleCount => 5;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. FindSwings
                var swings = FindSwings(bars, endIndex - 17, endIndex);

                // Primo swing usato (sL1) deve essere low
                int offset = 0;
                if (swings.Count > 0 && swings[0].IsHigh)
                    offset = 1;
                if (swings.Count < 5 + offset) return false;

                var sL1 = swings[offset];     // left shoulder low
                var sH1 = swings[offset + 1]; // high tra left shoulder e head
                var sL2 = swings[offset + 2]; // head low
                var sH2 = swings[offset + 3]; // high tra head e right shoulder
                var sL3 = swings[offset + 4]; // right shoulder low

                // Verifica tipi
                if (sL1.IsHigh || !sH1.IsHigh || sL2.IsHigh
                    || !sH2.IsHigh || sL3.IsHigh)
                    return false;

                // 2. Head e' il minimo piu' basso
                if (sL2.Price >= sL1.Price) return false;
                if (sL2.Price >= sL3.Price) return false;

                // 3. Shoulders a livello simile
                if (Math.Abs(sL1.Price - sL3.Price) >= 0.4 * localATR) return false;

                // 4. Neckline: linea che connette sH1 e sH2
                int denom = sH2.Index - sH1.Index;
                if (denom <= 0) return false;
                _necklineAtEnd = sH1.Price
                    + (sH2.Price - sH1.Price)
                      * (double)(endIndex - sH1.Index) / denom;
                _sL2Price = sL2.Price;

                // 5. Break: C[17] > necklineAtEnd
                if (C(bars, endIndex, 17) <= _necklineAtEnd) return false;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                if (!CheckPattern(bars, endIndex))
                    return new List<GhostCandle>();

                double patternHeight = _necklineAtEnd - _sL2Price;
                if (patternHeight <= 0)
                    return new List<GhostCandle>();

                double target = currentPrice + patternHeight;
                return GenerateGhostsToTarget(
                    bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.14 Mini H&S (Bear — Mirror) ----
        private class MiniHSPattern : PatternBase
        {
            private double _necklineAtEnd;
            private double _sH2Price;

            public MiniHSPattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Mini H&S";
            public override int RequiredCandles => 18;
            public override int GhostCandleCount => 5;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. FindSwings
                var swings = FindSwings(bars, endIndex - 17, endIndex);

                // Primo swing usato (sH1) deve essere high
                int offset = 0;
                if (swings.Count > 0 && !swings[0].IsHigh)
                    offset = 1;
                if (swings.Count < 5 + offset) return false;

                var sH1 = swings[offset];     // left shoulder high
                var sL1 = swings[offset + 1]; // low tra left shoulder e head
                var sH2 = swings[offset + 2]; // head high (piu' alto)
                var sL2 = swings[offset + 3]; // low tra head e right shoulder
                var sH3 = swings[offset + 4]; // right shoulder high

                // Verifica tipi
                if (!sH1.IsHigh || sL1.IsHigh || !sH2.IsHigh
                    || sL2.IsHigh || !sH3.IsHigh)
                    return false;

                // 2. Head e' il massimo piu' alto
                if (sH2.Price <= sH1.Price) return false;
                if (sH2.Price <= sH3.Price) return false;

                // 3. Shoulders a livello simile
                if (Math.Abs(sH1.Price - sH3.Price) >= 0.4 * localATR) return false;

                // 4. Neckline: linea che connette sL1 e sL2
                int denom = sL2.Index - sL1.Index;
                if (denom <= 0) return false;
                _necklineAtEnd = sL1.Price
                    + (sL2.Price - sL1.Price)
                      * (double)(endIndex - sL1.Index) / denom;
                _sH2Price = sH2.Price;

                // 5. Break: C[17] < necklineAtEnd
                if (C(bars, endIndex, 17) >= _necklineAtEnd) return false;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                if (!CheckPattern(bars, endIndex))
                    return new List<GhostCandle>();

                // necklineAtEnd < sH2Price → target negativo (bear)
                double patternHeight = _necklineAtEnd - _sH2Price;
                if (patternHeight >= 0)
                    return new List<GhostCandle>();

                double target = currentPrice + patternHeight;
                return GenerateGhostsToTarget(
                    bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ===== N.15 Pennant =====

        private class BullPennantPattern : PatternBase
        {
            public BullPennantPattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bull Pennant";
            public override int RequiredCandles => 8;
            public override int GhostCandleCount => 5;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. Impulso (c[0]-c[1]): C[1] - O[0] > 1.5 * localATR, entrambe bullish
                double impulse = C(bars, endIndex, 1) - O(bars, endIndex, 0);
                if (impulse <= 1.5 * localATR) return false;
                if (!IsBull(bars, endIndex, 0)) return false;
                if (!IsBull(bars, endIndex, 1)) return false;

                // 2. Triangolo simmetrico (c[2]-c[7]): H[7] < H[2] AND L[7] > L[2]
                if (H(bars, endIndex, 7) >= H(bars, endIndex, 2)) return false;
                if (L(bars, endIndex, 7) <= L(bars, endIndex, 2)) return false;

                // 3. Convergenza: range[2] > range[7] * 1.5
                if (Range(bars, endIndex, 2) <= Range(bars, endIndex, 7) * 1.5) return false;

                // Calcola consHigh, consLow e traccia posizione del max high (per cond. 4)
                double consHigh = double.MinValue;
                double consLow = double.MaxValue;
                int maxHIdx = 2;
                for (int i = 2; i <= 7; i++)
                {
                    double h = H(bars, endIndex, i);
                    double l = L(bars, endIndex, i);
                    if (h > consHigh) { consHigh = h; maxHIdx = i; }
                    consLow = Math.Min(consLow, l);
                }

                // 4. Convergenza progressiva: max(H[2..7]) e' a c[2] o c[3]
                if (maxHIdx != 2 && maxHIdx != 3) return false;

                // 5. Range contenuto: consHigh - consLow < impulse * 0.5
                if (consHigh - consLow >= impulse * 0.5) return false;

                // 6. Pennant nella parte alta dell'impulso: consLow > O[0] + impulse * 0.33
                if (consLow <= O(bars, endIndex, 0) + impulse * 0.33) return false;

                // 7. Close finale sopra mid del pennant: C[7] > (consHigh + consLow) / 2
                if (C(bars, endIndex, 7) <= (consHigh + consLow) / 2.0) return false;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                // Caso A: target = currentPrice + impulse (proiezione dell'impulso)
                double target = currentPrice + (C(bars, endIndex, 1) - O(bars, endIndex, 0));
                return GenerateGhostsToTarget(
                    bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        private class BearPennantPattern : PatternBase
        {
            public BearPennantPattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bear Pennant";
            public override int RequiredCandles => 8;
            public override int GhostCandleCount => 5;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. Impulso (c[0]-c[1]): O[0] - C[1] > 1.5 * localATR, entrambe bearish
                double impulse = O(bars, endIndex, 0) - C(bars, endIndex, 1);
                if (impulse <= 1.5 * localATR) return false;
                if (!IsBear(bars, endIndex, 0)) return false;
                if (!IsBear(bars, endIndex, 1)) return false;

                // 2. Triangolo simmetrico (c[2]-c[7]): H[7] < H[2] AND L[7] > L[2]
                // (stessa forma simmetrica sia nel bull che nel bear)
                if (H(bars, endIndex, 7) >= H(bars, endIndex, 2)) return false;
                if (L(bars, endIndex, 7) <= L(bars, endIndex, 2)) return false;

                // 3. Convergenza: range[2] > range[7] * 1.5
                if (Range(bars, endIndex, 2) <= Range(bars, endIndex, 7) * 1.5) return false;

                // Calcola consHigh, consLow e traccia posizione del min low (per cond. 4)
                double consHigh = double.MinValue;
                double consLow = double.MaxValue;
                int minLIdx = 2;
                for (int i = 2; i <= 7; i++)
                {
                    double h = H(bars, endIndex, i);
                    double l = L(bars, endIndex, i);
                    consHigh = Math.Max(consHigh, h);
                    if (l < consLow) { consLow = l; minLIdx = i; }
                }

                // 4. Convergenza progressiva: min(L[2..7]) e' a c[2] o c[3]
                if (minLIdx != 2 && minLIdx != 3) return false;

                // 5. Range contenuto: consHigh - consLow < impulse * 0.5
                if (consHigh - consLow >= impulse * 0.5) return false;

                // 6. Pennant nella parte bassa dell'impulso: consHigh < O[0] - impulse * 0.33
                if (consHigh >= O(bars, endIndex, 0) - impulse * 0.33) return false;

                // 7. Close finale sotto mid del pennant: C[7] < (consHigh + consLow) / 2
                if (C(bars, endIndex, 7) >= (consHigh + consLow) / 2.0) return false;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                // Caso A: target = currentPrice - impulse (proiezione al ribasso)
                double target = currentPrice - (O(bars, endIndex, 0) - C(bars, endIndex, 1));
                return GenerateGhostsToTarget(
                    bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ===== N.16 Broadening Wedge =====

        private class BullBroadeningWedgePattern : PatternBase
        {
            public BullBroadeningWedgePattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bull Broadening Wedge";
            public override int RequiredCandles => 10;
            public override int GhostCandleCount => 5;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. Highs declinanti: almeno 3 coppie (i, i+2) con H[i+2] < H[i]
                //    i pari in {0, 2, 4, 6} → coppie: (0,2), (2,4), (4,6), (6,8)
                int decliningHighs = 0;
                for (int i = 0; i <= 6; i += 2)
                {
                    if (H(bars, endIndex, i + 2) < H(bars, endIndex, i))
                        decliningHighs++;
                }
                if (decliningHighs < 3) return false;

                // 2. Lows declinanti: almeno 3 coppie (i, i+2) con L[i+2] < L[i]
                int decliningLows = 0;
                for (int i = 0; i <= 6; i += 2)
                {
                    if (L(bars, endIndex, i + 2) < L(bars, endIndex, i))
                        decliningLows++;
                }
                if (decliningLows < 3) return false;

                // 3. Divergenza: ampiezza formazione si allarga
                double maxH_start = Math.Max(H(bars, endIndex, 0),
                    Math.Max(H(bars, endIndex, 1), H(bars, endIndex, 2)));
                double minL_start = Math.Min(L(bars, endIndex, 0),
                    Math.Min(L(bars, endIndex, 1), L(bars, endIndex, 2)));
                double formationWidth_start = maxH_start - minL_start;

                double maxH_end = Math.Max(H(bars, endIndex, 7),
                    Math.Max(H(bars, endIndex, 8), H(bars, endIndex, 9)));
                double minL_end = Math.Min(L(bars, endIndex, 7),
                    Math.Min(L(bars, endIndex, 8), L(bars, endIndex, 9)));
                double formationWidth_end = maxH_end - minL_end;

                if (formationWidth_end <= formationWidth_start * 1.3) return false;

                // 4. Reversal: C[9] > H[8]
                if (C(bars, endIndex, 9) <= H(bars, endIndex, 8)) return false;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                // Caso A: target = currentPrice + (H[8] - L[8]) * 0.5
                double target = currentPrice + (H(bars, endIndex, 8) - L(bars, endIndex, 8)) * 0.5;
                return GenerateGhostsToTarget(
                    bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        private class BearBroadeningWedgePattern : PatternBase
        {
            public BearBroadeningWedgePattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bear Broadening Wedge";
            public override int RequiredCandles => 10;
            public override int GhostCandleCount => 5;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. Highs crescenti: almeno 3 coppie (i, i+2) con H[i+2] > H[i]
                int risingHighs = 0;
                for (int i = 0; i <= 6; i += 2)
                {
                    if (H(bars, endIndex, i + 2) > H(bars, endIndex, i))
                        risingHighs++;
                }
                if (risingHighs < 3) return false;

                // 2. Lows crescenti: almeno 3 coppie (i, i+2) con L[i+2] > L[i]
                int risingLows = 0;
                for (int i = 0; i <= 6; i += 2)
                {
                    if (L(bars, endIndex, i + 2) > L(bars, endIndex, i))
                        risingLows++;
                }
                if (risingLows < 3) return false;

                // 3. Divergenza: ampiezza formazione si allarga
                double maxH_start = Math.Max(H(bars, endIndex, 0),
                    Math.Max(H(bars, endIndex, 1), H(bars, endIndex, 2)));
                double minL_start = Math.Min(L(bars, endIndex, 0),
                    Math.Min(L(bars, endIndex, 1), L(bars, endIndex, 2)));
                double formationWidth_start = maxH_start - minL_start;

                double maxH_end = Math.Max(H(bars, endIndex, 7),
                    Math.Max(H(bars, endIndex, 8), H(bars, endIndex, 9)));
                double minL_end = Math.Min(L(bars, endIndex, 7),
                    Math.Min(L(bars, endIndex, 8), L(bars, endIndex, 9)));
                double formationWidth_end = maxH_end - minL_end;

                if (formationWidth_end <= formationWidth_start * 1.3) return false;

                // 4. Reversal: C[9] < L[8] (esplicito, non mirror meccanico)
                if (C(bars, endIndex, 9) >= L(bars, endIndex, 8)) return false;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                // Caso A: target = currentPrice - (H[8] - L[8]) * 0.5
                double target = currentPrice - (H(bars, endIndex, 8) - L(bars, endIndex, 8)) * 0.5;
                return GenerateGhostsToTarget(
                    bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.17 Bull Wolfe Wave ----
        private class BullWolfeWavePattern : PatternBase
        {
            private double _sH1Price;
            private double _sL3Price;

            public BullWolfeWavePattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bull Wolfe Wave";
            public override int RequiredCandles => 18;
            public override int GhostCandleCount => 5;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. FindSwings: need sL1, sH1, sL2, sH2, sL3
                var swings = FindSwings(bars, endIndex - 17, endIndex);
                int offset = 0;
                if (swings.Count > 0 && swings[0].IsHigh)
                    offset = 1;
                if (swings.Count < 5 + offset) return false;

                var sL1 = swings[offset];
                var sH1 = swings[offset + 1];
                var sL2 = swings[offset + 2];
                var sH2 = swings[offset + 3];
                var sL3 = swings[offset + 4];

                // Verify alternation: L, H, L, H, L
                if (sL1.IsHigh || !sH1.IsHigh || sL2.IsHigh || !sH2.IsHigh || sL3.IsHigh)
                    return false;

                // Point 5 must be one of the last 2-3 candles
                if (endIndex - sL3.Index > 2) return false;

                // 2. Lows decrescenti
                if (sL2.Price >= sL1.Price) return false;
                if (sL3.Price >= sL2.Price) return false;

                // 3. Highs decrescenti
                if (sH2.Price >= sH1.Price) return false;

                // 4. Allineamento 1-3-5: punto 3 sulla linea 1-5
                if (sL3.Index <= sL1.Index) return false;
                double expectedAt3 = sL1.Price + (sL3.Price - sL1.Price)
                    * (sL2.Index - sL1.Index) / (double)(sL3.Index - sL1.Index);
                if (Math.Abs(sL2.Price - expectedAt3) >= 0.3 * localATR) return false;

                // 5. Convergenza: gap tra trendlines si riduce
                if (Math.Abs(sH1.Price - sL1.Price) <= Math.Abs(sH2.Price - sL3.Price))
                    return false;

                _sH1Price = sH1.Price;
                _sL3Price = sL3.Price;
                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                // Caso B: refresh private fields
                if (!CheckPattern(bars, endIndex))
                    return new List<GhostCandle>();
                double target = currentPrice + (_sH1Price - _sL3Price) * 0.618;
                return GenerateGhostsToTarget(
                    bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.17 Bear Wolfe Wave ----
        private class BearWolfeWavePattern : PatternBase
        {
            private double _sL1Price;
            private double _sH3Price;

            public BearWolfeWavePattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bear Wolfe Wave";
            public override int RequiredCandles => 18;
            public override int GhostCandleCount => 5;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. FindSwings: need sH1, sL1, sH2, sL2, sH3 (mirror: starts with high)
                var swings = FindSwings(bars, endIndex - 17, endIndex);
                int offset = 0;
                if (swings.Count > 0 && !swings[0].IsHigh)
                    offset = 1;
                if (swings.Count < 5 + offset) return false;

                var sH1 = swings[offset];
                var sL1 = swings[offset + 1];
                var sH2 = swings[offset + 2];
                var sL2 = swings[offset + 3];
                var sH3 = swings[offset + 4];

                // Verify alternation: H, L, H, L, H
                if (!sH1.IsHigh || sL1.IsHigh || !sH2.IsHigh || sL2.IsHigh || !sH3.IsHigh)
                    return false;

                // Point 5 must be one of the last 2-3 candles
                if (endIndex - sH3.Index > 2) return false;

                // 2. Highs crescenti (mirror of lows decrescenti)
                if (sH2.Price <= sH1.Price) return false;
                if (sH3.Price <= sH2.Price) return false;

                // 3. Lows crescenti (mirror of highs decrescenti)
                if (sL2.Price <= sL1.Price) return false;

                // 4. Allineamento 1-3-5: punto 3 sulla linea 1-5
                if (sH3.Index <= sH1.Index) return false;
                double expectedAt3 = sH1.Price + (sH3.Price - sH1.Price)
                    * (sH2.Index - sH1.Index) / (double)(sH3.Index - sH1.Index);
                if (Math.Abs(sH2.Price - expectedAt3) >= 0.3 * localATR) return false;

                // 5. Convergenza: gap tra trendlines si riduce (Math.Abs per mirror safety)
                if (Math.Abs(sH1.Price - sL1.Price) <= Math.Abs(sH3.Price - sL2.Price))
                    return false;

                _sL1Price = sL1.Price;
                _sH3Price = sH3.Price;
                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                // Caso B: refresh private fields
                if (!CheckPattern(bars, endIndex))
                    return new List<GhostCandle>();
                // Mirror target: sL1(a low) - sH3(highest high) → negative
                double target = currentPrice + (_sL1Price - _sH3Price) * 0.618;
                return GenerateGhostsToTarget(
                    bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.18 Diamond Bottom ----
        private class DiamondBottomPattern : PatternBase
        {
            public DiamondBottomPattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Diamond Bottom";
            public override int RequiredCandles => 14;
            public override int GhostCandleCount => 5;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. Fase espansiva (c[0]-c[6])
                double startHigh = double.MinValue;
                for (int i = 0; i <= 2; i++)
                    startHigh = Math.Max(startHigh, H(bars, endIndex, i));

                double startLow = double.MaxValue;
                for (int i = 0; i <= 2; i++)
                    startLow = Math.Min(startLow, L(bars, endIndex, i));

                double expandHigh = double.MinValue;
                for (int i = 4; i <= 6; i++)
                    expandHigh = Math.Max(expandHigh, H(bars, endIndex, i));

                double expandLow = double.MaxValue;
                for (int i = 4; i <= 6; i++)
                    expandLow = Math.Min(expandLow, L(bars, endIndex, i));

                if (expandHigh <= startHigh) return false;
                if (expandLow >= startLow) return false;

                // 2. Fase contrattiva: wide (c[5]-c[8]) vs contract (c[10]-c[12])
                double wideHigh = double.MinValue;
                for (int i = 5; i <= 8; i++)
                    wideHigh = Math.Max(wideHigh, H(bars, endIndex, i));

                double wideLow = double.MaxValue;
                for (int i = 5; i <= 8; i++)
                    wideLow = Math.Min(wideLow, L(bars, endIndex, i));

                double contractHigh = double.MinValue;
                for (int i = 10; i <= 12; i++)
                    contractHigh = Math.Max(contractHigh, H(bars, endIndex, i));

                double contractLow = double.MaxValue;
                for (int i = 10; i <= 12; i++)
                    contractLow = Math.Min(contractLow, L(bars, endIndex, i));

                if (contractHigh >= wideHigh) return false;
                if (contractLow <= wideLow) return false;

                // 3. Forma diamante: zona centrale piu' ampia e significativa
                double wideRange = wideHigh - wideLow;
                double contractRange = contractHigh - contractLow;
                if (wideRange <= contractRange * 1.5) return false;
                if (wideRange <= 1.0 * localATR) return false;

                // 4. Break up
                if (C(bars, endIndex, 13) <= contractHigh) return false;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                // Caso A: recalculate wideHigh/wideLow directly
                double wideHigh = double.MinValue;
                for (int i = 5; i <= 8; i++)
                    wideHigh = Math.Max(wideHigh, H(bars, endIndex, i));

                double wideLow = double.MaxValue;
                for (int i = 5; i <= 8; i++)
                    wideLow = Math.Min(wideLow, L(bars, endIndex, i));

                double diamondHeight = wideHigh - wideLow;
                double localATR = LocalATR(endIndex);
                double cappedHeight = Math.Min(diamondHeight, 3.0 * localATR);
                double target = currentPrice + cappedHeight;
                return GenerateGhostsToTarget(
                    bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.18 Diamond Top ----
        private class DiamondTopPattern : PatternBase
        {
            public DiamondTopPattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Diamond Top";
            public override int RequiredCandles => 14;
            public override int GhostCandleCount => 5;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. Fase espansiva (c[0]-c[6]) — mirror: highs crescenti, lows decrescenti
                double startHigh = double.MinValue;
                for (int i = 0; i <= 2; i++)
                    startHigh = Math.Max(startHigh, H(bars, endIndex, i));

                double startLow = double.MaxValue;
                for (int i = 0; i <= 2; i++)
                    startLow = Math.Min(startLow, L(bars, endIndex, i));

                double expandHigh = double.MinValue;
                for (int i = 4; i <= 6; i++)
                    expandHigh = Math.Max(expandHigh, H(bars, endIndex, i));

                double expandLow = double.MaxValue;
                for (int i = 4; i <= 6; i++)
                    expandLow = Math.Min(expandLow, L(bars, endIndex, i));

                // Same expansion check — diamond shape is symmetric
                if (expandHigh <= startHigh) return false;
                if (expandLow >= startLow) return false;

                // 2. Fase contrattiva: wide (c[5]-c[8]) vs contract (c[10]-c[12])
                double wideHigh = double.MinValue;
                for (int i = 5; i <= 8; i++)
                    wideHigh = Math.Max(wideHigh, H(bars, endIndex, i));

                double wideLow = double.MaxValue;
                for (int i = 5; i <= 8; i++)
                    wideLow = Math.Min(wideLow, L(bars, endIndex, i));

                double contractHigh = double.MinValue;
                for (int i = 10; i <= 12; i++)
                    contractHigh = Math.Max(contractHigh, H(bars, endIndex, i));

                double contractLow = double.MaxValue;
                for (int i = 10; i <= 12; i++)
                    contractLow = Math.Min(contractLow, L(bars, endIndex, i));

                if (contractHigh >= wideHigh) return false;
                if (contractLow <= wideLow) return false;

                // 3. Forma diamante: zona centrale piu' ampia e significativa
                double wideRange = wideHigh - wideLow;
                double contractRange = contractHigh - contractLow;
                if (wideRange <= contractRange * 1.5) return false;
                if (wideRange <= 1.0 * localATR) return false;

                // 4. Break down (mirror: sotto contractLow)
                if (C(bars, endIndex, 13) >= contractLow) return false;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                // Caso A: recalculate wideHigh/wideLow directly
                // NON mirror H<->L: diamondHeight is geometric property, always positive
                double wideHigh = double.MinValue;
                for (int i = 5; i <= 8; i++)
                    wideHigh = Math.Max(wideHigh, H(bars, endIndex, i));

                double wideLow = double.MaxValue;
                for (int i = 5; i <= 8; i++)
                    wideLow = Math.Min(wideLow, L(bars, endIndex, i));

                double diamondHeight = wideHigh - wideLow;
                double localATR = LocalATR(endIndex);
                double cappedHeight = Math.Min(diamondHeight, 3.0 * localATR);
                double target = currentPrice - cappedHeight;
                return GenerateGhostsToTarget(
                    bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.19 Bull Three Drives ----
        private class BullThreeDrivesPattern : PatternBase
        {
            private double _sL1Price;
            private double _sL3Price;

            public BullThreeDrivesPattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bull Three Drives";
            public override int RequiredCandles => 18;
            public override int GhostCandleCount => 5;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. FindSwings — need sH0, sL1, sH1, sL2, sH2, sL3 (starting with high)
                var swings = FindSwings(bars, endIndex - 17, endIndex);
                int offset = swings.Count > 0 && !swings[0].IsHigh ? 1 : 0;
                if (swings.Count < 6 + offset) return false;

                var sH0 = swings[offset];
                var sL1 = swings[offset + 1];
                var sH1 = swings[offset + 2];
                var sL2 = swings[offset + 3];
                var sH2 = swings[offset + 4];
                var sL3 = swings[offset + 5];

                if (!sH0.IsHigh || sL1.IsHigh || !sH1.IsHigh ||
                    sL2.IsHigh || !sH2.IsHigh || sL3.IsHigh)
                    return false;

                // 2. Three progressive drives (decreasing lows)
                if (sL2.Price >= sL1.Price) return false;
                if (sL3.Price >= sL2.Price) return false;

                // 3. Temporal symmetry
                int dist1 = sL2.Index - sL1.Index;
                int dist2 = sL3.Index - sL2.Index;
                if (dist1 <= 0) return false;
                if (Math.Abs(dist1 - dist2) > dist1 * 0.3) return false;

                // 4. Guard division by zero
                double denom1 = sH0.Price - sL1.Price;
                double denom2 = sH1.Price - sL2.Price;
                if (denom1 < 0.1 * localATR) return false;
                if (denom2 < 0.1 * localATR) return false;

                // 5. Fib retracement correction 1
                if (!FibCheck((sH1.Price - sL1.Price) / denom1, 0.618, 0.1)) return false;

                // 6. Fib extension drive 2
                if (!FibCheck((sH1.Price - sL2.Price) / denom1, 1.272, 0.15)) return false;

                // 7. Fib retracement correction 2
                if (!FibCheck((sH2.Price - sL2.Price) / denom2, 0.618, 0.1)) return false;

                // 8. Fib extension drive 3
                if (!FibCheck((sH2.Price - sL3.Price) / denom2, 1.272, 0.15)) return false;

                // 9. Freshness third drive
                if (sL3.Index < endIndex - 4) return false;

                // 10. Reaction
                if (C(bars, endIndex, 17) <= sL3.Price + 0.3 * localATR) return false;

                _sL1Price = sL1.Price;
                _sL3Price = sL3.Price;
                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                if (!CheckPattern(bars, endIndex))
                    return new List<GhostCandle>();
                double target = currentPrice + (_sL1Price - _sL3Price) * 0.618;
                return GenerateGhostsToTarget(
                    bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.19 Bear Three Drives ----
        private class BearThreeDrivesPattern : PatternBase
        {
            private double _sH1Price;
            private double _sH3Price;

            public BearThreeDrivesPattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bear Three Drives";
            public override int RequiredCandles => 18;
            public override int GhostCandleCount => 5;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. FindSwings — need sL0, sH1, sL1, sH2, sL2, sH3 (starting with low)
                var swings = FindSwings(bars, endIndex - 17, endIndex);
                int offset = swings.Count > 0 && swings[0].IsHigh ? 1 : 0;
                if (swings.Count < 6 + offset) return false;

                var sL0 = swings[offset];
                var sH1 = swings[offset + 1];
                var sL1 = swings[offset + 2];
                var sH2 = swings[offset + 3];
                var sL2 = swings[offset + 4];
                var sH3 = swings[offset + 5];

                if (sL0.IsHigh || !sH1.IsHigh || sL1.IsHigh ||
                    !sH2.IsHigh || sL2.IsHigh || !sH3.IsHigh)
                    return false;

                // 2. Three progressive drives (increasing highs)
                if (sH2.Price <= sH1.Price) return false;
                if (sH3.Price <= sH2.Price) return false;

                // 3. Temporal symmetry
                int dist1 = sH2.Index - sH1.Index;
                int dist2 = sH3.Index - sH2.Index;
                if (dist1 <= 0) return false;
                if (Math.Abs(dist1 - dist2) > dist1 * 0.3) return false;

                // 4. Guard division by zero
                double denom1 = sH1.Price - sL0.Price;
                double denom2 = sH2.Price - sL1.Price;
                if (denom1 < 0.1 * localATR) return false;
                if (denom2 < 0.1 * localATR) return false;

                // 5. Fib retracement correction 1
                if (!FibCheck((sH1.Price - sL1.Price) / denom1, 0.618, 0.1)) return false;

                // 6. Fib extension drive 2
                if (!FibCheck((sH2.Price - sL1.Price) / denom1, 1.272, 0.15)) return false;

                // 7. Fib retracement correction 2
                if (!FibCheck((sH2.Price - sL2.Price) / denom2, 0.618, 0.1)) return false;

                // 8. Fib extension drive 3
                if (!FibCheck((sH3.Price - sL2.Price) / denom2, 1.272, 0.15)) return false;

                // 9. Freshness third drive
                if (sH3.Index < endIndex - 4) return false;

                // 10. Reaction (bear: price drops below drive 3)
                if (C(bars, endIndex, 17) >= sH3.Price - 0.3 * localATR) return false;

                _sH1Price = sH1.Price;
                _sH3Price = sH3.Price;
                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                if (!CheckPattern(bars, endIndex))
                    return new List<GhostCandle>();
                double target = currentPrice + (_sH1Price - _sH3Price) * 0.618;
                return GenerateGhostsToTarget(
                    bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.20 Pipe Bottom ----
        private class PipeBottomPattern : PatternBase
        {
            public PipeBottomPattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Pipe Bottom";
            public override int RequiredCandles => 4;
            public override int GhostCandleCount => 4;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. Big bearish candle (c[0])
                if (!IsBear(bars, endIndex, 0)) return false;
                if (Body(bars, endIndex, 0) <= 0.7 * Range(bars, endIndex, 0)) return false;
                if (Range(bars, endIndex, 0) <= 1.5 * localATR) return false;

                // 2. Big bullish candle (c[1])
                if (!IsBull(bars, endIndex, 1)) return false;
                if (Body(bars, endIndex, 1) <= 0.7 * Range(bars, endIndex, 1)) return false;
                if (Range(bars, endIndex, 1) <= 1.5 * localATR) return false;

                // 3. Lows at same level
                if (Math.Abs(L(bars, endIndex, 0) - L(bars, endIndex, 1)) >= 0.2 * localATR)
                    return false;

                // 4. Close of second above open of first
                if (C(bars, endIndex, 1) <= O(bars, endIndex, 0)) return false;

                // 5. Follow-through
                if (!IsBull(bars, endIndex, 2) && !IsBull(bars, endIndex, 3)) return false;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                double localATR = LocalATR(endIndex);
                double pipeRange = Math.Min(
                    Math.Max(Range(bars, endIndex, 0), Range(bars, endIndex, 1)),
                    2.0 * localATR);
                double target = currentPrice + pipeRange;
                return GenerateGhostsToTarget(
                    bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.20 Pipe Top ----
        private class PipeTopPattern : PatternBase
        {
            public PipeTopPattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Pipe Top";
            public override int RequiredCandles => 4;
            public override int GhostCandleCount => 4;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. Big bullish candle (c[0])
                if (!IsBull(bars, endIndex, 0)) return false;
                if (Body(bars, endIndex, 0) <= 0.7 * Range(bars, endIndex, 0)) return false;
                if (Range(bars, endIndex, 0) <= 1.5 * localATR) return false;

                // 2. Big bearish candle (c[1])
                if (!IsBear(bars, endIndex, 1)) return false;
                if (Body(bars, endIndex, 1) <= 0.7 * Range(bars, endIndex, 1)) return false;
                if (Range(bars, endIndex, 1) <= 1.5 * localATR) return false;

                // 3. Highs at same level
                if (Math.Abs(H(bars, endIndex, 0) - H(bars, endIndex, 1)) >= 0.2 * localATR)
                    return false;

                // 4. Close of second below open of first
                if (C(bars, endIndex, 1) >= O(bars, endIndex, 0)) return false;

                // 5. Follow-through (bearish)
                if (!IsBear(bars, endIndex, 2) && !IsBear(bars, endIndex, 3)) return false;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                double localATR = LocalATR(endIndex);
                double pipeRange = Math.Min(
                    Math.Max(Range(bars, endIndex, 0), Range(bars, endIndex, 1)),
                    2.0 * localATR);
                double target = currentPrice - pipeRange;
                return GenerateGhostsToTarget(
                    bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.21 Bull Gartley ----
        private class BullGartleyPattern : PatternBase
        {
            private double _aPrice;
            private double _bPrice;

            public BullGartleyPattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bull Gartley";
            public override int RequiredCandles => 20;
            public override int GhostCandleCount => 5;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. FindSwings — need X(low), A(high), B(low), C(high), D(low)
                var swings = FindSwings(bars, endIndex - 19, endIndex);
                int offset = 0;
                if (swings.Count > 0 && swings[0].IsHigh)
                    offset = 1;
                if (swings.Count < 5 + offset) return false;

                var X = swings[offset];
                var A = swings[offset + 1];
                var B = swings[offset + 2];
                var C = swings[offset + 3];
                var D = swings[offset + 4];

                if (X.IsHigh || !A.IsHigh || B.IsHigh || !C.IsHigh || D.IsHigh) return false;

                // 2. Guard XA (divisione per zero)
                double XA = A.Price - X.Price;
                if (XA < 0.1 * localATR) return false;

                // 3. Ampiezza minima XA
                if (XA < 1.0 * localATR) return false;

                // 4. Retracement AB (B ritraccia 61.8% di XA)
                double AB = A.Price - B.Price;
                if (!FibCheck(AB / XA, 0.618, 0.05)) return false;

                // Guard AB per divisioni successive
                if (AB < 0.1 * localATR) return false;

                // 5. Retracement BC (38.2% - 88.6% di AB)
                double BC = C.Price - B.Price;
                double bcRatio = BC / AB;
                if (bcRatio < 0.382 || bcRatio > 0.886) return false;

                // Guard BC per divisione successiva
                if (BC < 0.1 * localATR) return false;

                // 6. Retracement AD (D raggiunge 78.6% di XA)
                double AD = A.Price - D.Price;
                if (!FibCheck(AD / XA, 0.786, 0.05)) return false;

                // 7. Rapporto CD/BC (1.272 - 1.618)
                double CD = C.Price - D.Price;
                double cdBcRatio = CD / BC;
                if (cdBcRatio < 1.272 || cdBcRatio > 1.618) return false;

                // 8. D non viola X
                if (D.Price <= X.Price) return false;

                // 9. D ancorato a finestra (ultime 2 candele)
                if (D.Index < endIndex - 1) return false;

                _aPrice = A.Price;
                _bPrice = B.Price;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                if (!CheckPattern(bars, endIndex))
                    return new List<GhostCandle>();
                double target = currentPrice + (_aPrice - _bPrice) * 0.618;
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.21 Bear Gartley ----
        private class BearGartleyPattern : PatternBase
        {
            private double _aPrice;
            private double _bPrice;

            public BearGartleyPattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bear Gartley";
            public override int RequiredCandles => 20;
            public override int GhostCandleCount => 5;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. FindSwings — need X(high), A(low), B(high), C(low), D(high)
                var swings = FindSwings(bars, endIndex - 19, endIndex);
                int offset = 0;
                if (swings.Count > 0 && !swings[0].IsHigh)
                    offset = 1;
                if (swings.Count < 5 + offset) return false;

                var X = swings[offset];
                var A = swings[offset + 1];
                var B = swings[offset + 2];
                var C = swings[offset + 3];
                var D = swings[offset + 4];

                if (!X.IsHigh || A.IsHigh || !B.IsHigh || C.IsHigh || !D.IsHigh) return false;

                // 2. Guard XA (divisione per zero)
                double XA = X.Price - A.Price;
                if (XA < 0.1 * localATR) return false;

                // 3. Ampiezza minima XA
                if (XA < 1.0 * localATR) return false;

                // 4. Retracement AB (B ritraccia 61.8% di XA)
                double AB = B.Price - A.Price;
                if (!FibCheck(AB / XA, 0.618, 0.05)) return false;

                // Guard AB per divisioni successive
                if (AB < 0.1 * localATR) return false;

                // 5. Retracement BC (38.2% - 88.6% di AB)
                double BC = B.Price - C.Price;
                double bcRatio = BC / AB;
                if (bcRatio < 0.382 || bcRatio > 0.886) return false;

                // Guard BC per divisione successiva
                if (BC < 0.1 * localATR) return false;

                // 6. Retracement AD (D raggiunge 78.6% di XA)
                double AD = D.Price - A.Price;
                if (!FibCheck(AD / XA, 0.786, 0.05)) return false;

                // 7. Rapporto CD/BC (1.272 - 1.618)
                double CD = D.Price - C.Price;
                double cdBcRatio = CD / BC;
                if (cdBcRatio < 1.272 || cdBcRatio > 1.618) return false;

                // 8. D non viola X
                if (D.Price >= X.Price) return false;

                // 9. D ancorato a finestra (ultime 2 candele)
                if (D.Index < endIndex - 1) return false;

                _aPrice = A.Price;
                _bPrice = B.Price;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                if (!CheckPattern(bars, endIndex))
                    return new List<GhostCandle>();
                double target = currentPrice + (_aPrice - _bPrice) * 0.618;
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.22 Bull Bat ----
        private class BullBatPattern : PatternBase
        {
            private double _aPrice;
            private double _bPrice;

            public BullBatPattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bull Bat";
            public override int RequiredCandles => 20;
            public override int GhostCandleCount => 5;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. FindSwings — need X(low), A(high), B(low), C(high), D(low)
                var swings = FindSwings(bars, endIndex - 19, endIndex);
                int offset = 0;
                if (swings.Count > 0 && swings[0].IsHigh)
                    offset = 1;
                if (swings.Count < 5 + offset) return false;

                var X = swings[offset];
                var A = swings[offset + 1];
                var B = swings[offset + 2];
                var C = swings[offset + 3];
                var D = swings[offset + 4];

                if (X.IsHigh || !A.IsHigh || B.IsHigh || !C.IsHigh || D.IsHigh) return false;

                // 2. Guard XA (divisione per zero)
                double XA = A.Price - X.Price;
                if (XA < 0.1 * localATR) return false;

                // 3. Ampiezza minima XA
                if (XA < 1.0 * localATR) return false;

                // 4. Retracement AB (38.2% - 50% di XA)
                double AB = A.Price - B.Price;
                double abRatio = AB / XA;
                if (abRatio < 0.382 || abRatio > 0.50) return false;

                // Guard AB per divisioni successive
                if (AB < 0.1 * localATR) return false;

                // 5. Retracement BC (38.2% - 88.6% di AB)
                double BC = C.Price - B.Price;
                double bcRatio = BC / AB;
                if (bcRatio < 0.382 || bcRatio > 0.886) return false;

                // Guard BC per divisione successiva
                if (BC < 0.1 * localATR) return false;

                // 6. Retracement AD (D raggiunge 88.6% di XA)
                double AD = A.Price - D.Price;
                if (!FibCheck(AD / XA, 0.886, 0.05)) return false;

                // 7. Rapporto CD/BC (1.618 - 2.618)
                double CD = C.Price - D.Price;
                double cdBcRatio = CD / BC;
                if (cdBcRatio < 1.618 || cdBcRatio > 2.618) return false;

                // 8. D non viola X
                if (D.Price <= X.Price) return false;

                // 9. D ancorato a finestra (ultime 2 candele)
                if (D.Index < endIndex - 1) return false;

                _aPrice = A.Price;
                _bPrice = B.Price;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                if (!CheckPattern(bars, endIndex))
                    return new List<GhostCandle>();
                double target = currentPrice + (_aPrice - _bPrice) * 0.50;
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.22 Bear Bat ----
        private class BearBatPattern : PatternBase
        {
            private double _aPrice;
            private double _bPrice;

            public BearBatPattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bear Bat";
            public override int RequiredCandles => 20;
            public override int GhostCandleCount => 5;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. FindSwings — need X(high), A(low), B(high), C(low), D(high)
                var swings = FindSwings(bars, endIndex - 19, endIndex);
                int offset = 0;
                if (swings.Count > 0 && !swings[0].IsHigh)
                    offset = 1;
                if (swings.Count < 5 + offset) return false;

                var X = swings[offset];
                var A = swings[offset + 1];
                var B = swings[offset + 2];
                var C = swings[offset + 3];
                var D = swings[offset + 4];

                if (!X.IsHigh || A.IsHigh || !B.IsHigh || C.IsHigh || !D.IsHigh) return false;

                // 2. Guard XA (divisione per zero)
                double XA = X.Price - A.Price;
                if (XA < 0.1 * localATR) return false;

                // 3. Ampiezza minima XA
                if (XA < 1.0 * localATR) return false;

                // 4. Retracement AB (38.2% - 50% di XA)
                double AB = B.Price - A.Price;
                double abRatio = AB / XA;
                if (abRatio < 0.382 || abRatio > 0.50) return false;

                // Guard AB per divisioni successive
                if (AB < 0.1 * localATR) return false;

                // 5. Retracement BC (38.2% - 88.6% di AB)
                double BC = B.Price - C.Price;
                double bcRatio = BC / AB;
                if (bcRatio < 0.382 || bcRatio > 0.886) return false;

                // Guard BC per divisione successiva
                if (BC < 0.1 * localATR) return false;

                // 6. Retracement AD (D raggiunge 88.6% di XA)
                double AD = D.Price - A.Price;
                if (!FibCheck(AD / XA, 0.886, 0.05)) return false;

                // 7. Rapporto CD/BC (1.618 - 2.618)
                double CD = D.Price - C.Price;
                double cdBcRatio = CD / BC;
                if (cdBcRatio < 1.618 || cdBcRatio > 2.618) return false;

                // 8. D non viola X
                if (D.Price >= X.Price) return false;

                // 9. D ancorato a finestra (ultime 2 candele)
                if (D.Index < endIndex - 1) return false;

                _aPrice = A.Price;
                _bPrice = B.Price;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                if (!CheckPattern(bars, endIndex))
                    return new List<GhostCandle>();
                double target = currentPrice + (_aPrice - _bPrice) * 0.50;
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.23 Bull Crab ----
        private class BullCrabPattern : PatternBase
        {
            private double _aPrice;
            private double _xPrice;

            public BullCrabPattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bull Crab";
            public override int RequiredCandles => 20;
            public override int GhostCandleCount => 5;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. FindSwings — need X(low), A(high), B(low), C(high), D(low)
                var swings = FindSwings(bars, endIndex - 19, endIndex);
                int offset = 0;
                if (swings.Count > 0 && swings[0].IsHigh)
                    offset = 1;
                if (swings.Count < 5 + offset) return false;

                var X = swings[offset];
                var A = swings[offset + 1];
                var B = swings[offset + 2];
                var C = swings[offset + 3];
                var D = swings[offset + 4];

                if (X.IsHigh || !A.IsHigh || B.IsHigh || !C.IsHigh || D.IsHigh) return false;

                // 2. Guard XA (divisione per zero)
                double XA = A.Price - X.Price;
                if (XA < 0.1 * localATR) return false;

                // 3. Ampiezza minima XA
                if (XA < 1.0 * localATR) return false;

                // 4. Retracement AB — guard AB anti divisione-per-zero
                double AB = A.Price - B.Price;
                if (AB < 0.1 * localATR) return false;
                double abRatio = AB / XA;
                if (abRatio < 0.382 || abRatio > 0.618) return false;

                // 5. Retracement BC (38.2% - 88.6% di AB)
                double BC = C.Price - B.Price;
                double bcRatio = BC / AB;
                if (bcRatio < 0.382 || bcRatio > 0.886) return false;

                // Guard BC per divisione successiva
                if (BC < 0.1 * localATR) return false;

                // 6. Proporzione CD/BC (2.24 - 3.618)
                double CD = C.Price - D.Price;
                double cdBcRatio = CD / BC;
                if (cdBcRatio < 2.24 || cdBcRatio > 3.618) return false;

                // 7. Estensione AD (D estende al 161.8% di XA — D sotto X)
                double AD = A.Price - D.Price;
                if (!FibCheck(AD / XA, 1.618, 0.10)) return false;

                // 8. D ancorato a finestra (ultime 2 candele)
                if (D.Index < endIndex - 1) return false;

                _aPrice = A.Price;
                _xPrice = X.Price;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                if (!CheckPattern(bars, endIndex))
                    return new List<GhostCandle>();
                double target = currentPrice + (_aPrice - _xPrice) * 0.618;
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.23 Bear Crab ----
        private class BearCrabPattern : PatternBase
        {
            private double _aPrice;
            private double _xPrice;

            public BearCrabPattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bear Crab";
            public override int RequiredCandles => 20;
            public override int GhostCandleCount => 5;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. FindSwings — need X(high), A(low), B(high), C(low), D(high)
                var swings = FindSwings(bars, endIndex - 19, endIndex);
                int offset = 0;
                if (swings.Count > 0 && !swings[0].IsHigh)
                    offset = 1;
                if (swings.Count < 5 + offset) return false;

                var X = swings[offset];
                var A = swings[offset + 1];
                var B = swings[offset + 2];
                var C = swings[offset + 3];
                var D = swings[offset + 4];

                if (!X.IsHigh || A.IsHigh || !B.IsHigh || C.IsHigh || !D.IsHigh) return false;

                // 2. Guard XA (divisione per zero)
                double XA = X.Price - A.Price;
                if (XA < 0.1 * localATR) return false;

                // 3. Ampiezza minima XA
                if (XA < 1.0 * localATR) return false;

                // 4. Retracement AB — guard AB anti divisione-per-zero
                double AB = B.Price - A.Price;
                if (AB < 0.1 * localATR) return false;
                double abRatio = AB / XA;
                if (abRatio < 0.382 || abRatio > 0.618) return false;

                // 5. Retracement BC (38.2% - 88.6% di AB)
                double BC = B.Price - C.Price;
                double bcRatio = BC / AB;
                if (bcRatio < 0.382 || bcRatio > 0.886) return false;

                // Guard BC per divisione successiva
                if (BC < 0.1 * localATR) return false;

                // 6. Proporzione CD/BC (2.24 - 3.618)
                double CD = D.Price - C.Price;
                double cdBcRatio = CD / BC;
                if (cdBcRatio < 2.24 || cdBcRatio > 3.618) return false;

                // 7. Estensione AD (D estende al 161.8% di XA — D sopra X)
                double AD = D.Price - A.Price;
                if (!FibCheck(AD / XA, 1.618, 0.10)) return false;

                // 8. D ancorato a finestra (ultime 2 candele)
                if (D.Index < endIndex - 1) return false;

                _aPrice = A.Price;
                _xPrice = X.Price;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                if (!CheckPattern(bars, endIndex))
                    return new List<GhostCandle>();
                double target = currentPrice + (_aPrice - _xPrice) * 0.618;
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.24 Bull Butterfly ----
        private class BullButterflyPattern : PatternBase
        {
            private double _aPrice;
            private double _xPrice;

            public BullButterflyPattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bull Butterfly";
            public override int RequiredCandles => 20;
            public override int GhostCandleCount => 5;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. FindSwings — need X(low), A(high), B(low), C(high), D(low)
                var swings = FindSwings(bars, endIndex - 19, endIndex);
                int offset = 0;
                if (swings.Count > 0 && swings[0].IsHigh)
                    offset = 1;
                if (swings.Count < 5 + offset) return false;

                var X = swings[offset];
                var A = swings[offset + 1];
                var B = swings[offset + 2];
                var C = swings[offset + 3];
                var D = swings[offset + 4];

                if (X.IsHigh || !A.IsHigh || B.IsHigh || !C.IsHigh || D.IsHigh) return false;

                // 2. Retracement AB — guard XA e AB anti divisione-per-zero
                double XA = A.Price - X.Price;
                if (XA < 0.1 * localATR) return false;
                double AB = A.Price - B.Price;
                if (AB < 0.1 * localATR) return false;
                if (!FibCheck(AB / XA, 0.786, 0.05)) return false;

                // 3. Retracement BC (38.2% - 88.6% di AB)
                double BC = C.Price - B.Price;
                double bcRatio = BC / AB;
                if (bcRatio < 0.382 || bcRatio > 0.886) return false;

                // Guard BC per divisione successiva
                if (BC < 0.1 * localATR) return false;

                // 4. Proporzione CD/BC (1.618 - 2.618)
                double CD = C.Price - D.Price;
                double cdBcRatio = CD / BC;
                if (cdBcRatio < 1.618 || cdBcRatio > 2.618) return false;

                // 5. Estensione AD (D estende tra 127% e 161.8% di XA — D sotto X)
                double AD = A.Price - D.Price;
                double adRatio = AD / XA;
                if (adRatio < 1.27 || adRatio > 1.618) return false;

                // 6. Ampiezza minima XA
                if (XA < 1.0 * localATR) return false;

                // 7. D ancorato a finestra (ultime 2 candele)
                if (D.Index < endIndex - 1) return false;

                _aPrice = A.Price;
                _xPrice = X.Price;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                if (!CheckPattern(bars, endIndex))
                    return new List<GhostCandle>();
                double target = currentPrice + (_aPrice - _xPrice) * 0.618;
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.24 Bear Butterfly ----
        private class BearButterflyPattern : PatternBase
        {
            private double _aPrice;
            private double _xPrice;

            public BearButterflyPattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bear Butterfly";
            public override int RequiredCandles => 20;
            public override int GhostCandleCount => 5;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. FindSwings — need X(high), A(low), B(high), C(low), D(high)
                var swings = FindSwings(bars, endIndex - 19, endIndex);
                int offset = 0;
                if (swings.Count > 0 && !swings[0].IsHigh)
                    offset = 1;
                if (swings.Count < 5 + offset) return false;

                var X = swings[offset];
                var A = swings[offset + 1];
                var B = swings[offset + 2];
                var C = swings[offset + 3];
                var D = swings[offset + 4];

                if (!X.IsHigh || A.IsHigh || !B.IsHigh || C.IsHigh || !D.IsHigh) return false;

                // 2. Retracement AB — guard XA e AB anti divisione-per-zero
                double XA = X.Price - A.Price;
                if (XA < 0.1 * localATR) return false;
                double AB = B.Price - A.Price;
                if (AB < 0.1 * localATR) return false;
                if (!FibCheck(AB / XA, 0.786, 0.05)) return false;

                // 3. Retracement BC (38.2% - 88.6% di AB)
                double BC = B.Price - C.Price;
                double bcRatio = BC / AB;
                if (bcRatio < 0.382 || bcRatio > 0.886) return false;

                // Guard BC per divisione successiva
                if (BC < 0.1 * localATR) return false;

                // 4. Proporzione CD/BC (1.618 - 2.618)
                double CD = D.Price - C.Price;
                double cdBcRatio = CD / BC;
                if (cdBcRatio < 1.618 || cdBcRatio > 2.618) return false;

                // 5. Estensione AD (D estende tra 127% e 161.8% di XA — D sopra X)
                double AD = D.Price - A.Price;
                double adRatio = AD / XA;
                if (adRatio < 1.27 || adRatio > 1.618) return false;

                // 6. Ampiezza minima XA
                if (XA < 1.0 * localATR) return false;

                // 7. D ancorato a finestra (ultime 2 candele)
                if (D.Index < endIndex - 1) return false;

                _aPrice = A.Price;
                _xPrice = X.Price;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                if (!CheckPattern(bars, endIndex))
                    return new List<GhostCandle>();
                double target = currentPrice + (_aPrice - _xPrice) * 0.618;
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.25 Bull Volume Climax ----
        private class BullVolumeClimaxPattern : PatternBase
        {
            public BullVolumeClimaxPattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bull Vol Climax";
            public override int RequiredCandles => 6;
            public override int GhostCandleCount => 5;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. Media volume locale c[0]..c[3]
                double avgVol = 0;
                for (int i = 0; i <= 3; i++)
                    avgVol += V(bars, endIndex, i);
                avgVol /= 4.0;
                if (avgVol <= 0) return false;

                // 2. Trend ribassista c[0]-c[3]
                if (C(bars, endIndex, 3) >= C(bars, endIndex, 0) - 0.5 * localATR) return false;
                int bearCount = 0;
                for (int i = 0; i <= 3; i++)
                    if (IsBear(bars, endIndex, i)) bearCount++;
                if (bearCount < 3) return false;

                // 3. Climax c[4]: bear, volume esplosivo, range ampio
                if (!IsBear(bars, endIndex, 4)) return false;
                if (V(bars, endIndex, 4) <= 2.0 * avgVol) return false;
                if (Range(bars, endIndex, 4) <= 1.5 * localATR) return false;

                // 4. Reversal c[5]: bull, chiude sopra mid climax, volume sopra media
                if (!IsBull(bars, endIndex, 5)) return false;
                if (C(bars, endIndex, 5) <= Mid(bars, endIndex, 4)) return false;
                if (V(bars, endIndex, 5) <= avgVol) return false;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                double target = currentPrice + (H(bars, endIndex, 4) - L(bars, endIndex, 4));
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.25 Bear Volume Climax ----
        private class BearVolumeClimaxPattern : PatternBase
        {
            public BearVolumeClimaxPattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bear Vol Climax";
            public override int RequiredCandles => 6;
            public override int GhostCandleCount => 5;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. Media volume locale c[0]..c[3]
                double avgVol = 0;
                for (int i = 0; i <= 3; i++)
                    avgVol += V(bars, endIndex, i);
                avgVol /= 4.0;
                if (avgVol <= 0) return false;

                // 2. Trend rialzista c[0]-c[3] (mirror)
                if (C(bars, endIndex, 3) <= C(bars, endIndex, 0) + 0.5 * localATR) return false;
                int bullCount = 0;
                for (int i = 0; i <= 3; i++)
                    if (IsBull(bars, endIndex, i)) bullCount++;
                if (bullCount < 3) return false;

                // 3. Climax c[4]: bull (buying climax), volume esplosivo, range ampio
                if (!IsBull(bars, endIndex, 4)) return false;
                if (V(bars, endIndex, 4) <= 2.0 * avgVol) return false;
                if (Range(bars, endIndex, 4) <= 1.5 * localATR) return false;

                // 4. Reversal c[5]: bear, chiude sotto mid climax, volume sopra media
                if (!IsBear(bars, endIndex, 5)) return false;
                if (C(bars, endIndex, 5) >= Mid(bars, endIndex, 4)) return false;
                if (V(bars, endIndex, 5) <= avgVol) return false;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                double target = currentPrice + (L(bars, endIndex, 4) - H(bars, endIndex, 4));
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.26 Bull Three Pushes ----
        private class BullThreePushesPattern : PatternBase
        {
            private double _push1Price;
            private double _push3Price;

            public BullThreePushesPattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bull Three Pushes";
            public override int RequiredCandles => 12;
            public override int GhostCandleCount => 5;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. FindSwings — prendere ultimi 3 swing lows
                var swings = FindSwings(bars, endIndex - 11, endIndex);
                var lows = new List<SwingPoint>();
                for (int i = 0; i < swings.Count; i++)
                    if (!swings[i].IsHigh) lows.Add(swings[i]);
                if (lows.Count < 3) return false;

                var push1 = lows[lows.Count - 3];
                var push2 = lows[lows.Count - 2];
                var push3 = lows[lows.Count - 1];

                // 2. Tre minimi decrescenti
                if (push2.Price >= push1.Price) return false;
                if (push3.Price >= push2.Price) return false;

                // 3. Momentum decrescente
                if (Math.Abs(push1.Price - push2.Price) <= Math.Abs(push2.Price - push3.Price)) return false;

                // 4. Divergenza di volatilita'
                int push1Idx = push1.Index - (endIndex - RequiredCandles + 1);
                int push3Idx = push3.Index - (endIndex - RequiredCandles + 1);
                double rangePush3 = H(bars, endIndex, push3Idx) - L(bars, endIndex, push3Idx);
                double rangePush1 = H(bars, endIndex, push1Idx) - L(bars, endIndex, push1Idx);
                if (rangePush3 >= rangePush1) return false;

                // 5. Reversal: reazione significativa dal terzo push
                if (C(bars, endIndex, 11) <= push3.Price + 0.5 * localATR) return false;

                _push1Price = push1.Price;
                _push3Price = push3.Price;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                // Caso B: rinfresca i campi privati
                if (!CheckPattern(bars, endIndex))
                    return new List<GhostCandle>();
                double target = currentPrice + (_push1Price - _push3Price) * 0.618;
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.26 Bear Three Pushes ----
        private class BearThreePushesPattern : PatternBase
        {
            private double _push1Price;
            private double _push3Price;

            public BearThreePushesPattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bear Three Pushes";
            public override int RequiredCandles => 12;
            public override int GhostCandleCount => 5;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. FindSwings — prendere ultimi 3 swing highs
                var swings = FindSwings(bars, endIndex - 11, endIndex);
                var highs = new List<SwingPoint>();
                for (int i = 0; i < swings.Count; i++)
                    if (swings[i].IsHigh) highs.Add(swings[i]);
                if (highs.Count < 3) return false;

                var push1 = highs[highs.Count - 3];
                var push2 = highs[highs.Count - 2];
                var push3 = highs[highs.Count - 1];

                // 2. Tre massimi crescenti (mirror)
                if (push2.Price <= push1.Price) return false;
                if (push3.Price <= push2.Price) return false;

                // 3. Momentum decrescente
                if (Math.Abs(push1.Price - push2.Price) <= Math.Abs(push2.Price - push3.Price)) return false;

                // 4. Divergenza di volatilita' (invariante al mirror)
                int push1Idx = push1.Index - (endIndex - RequiredCandles + 1);
                int push3Idx = push3.Index - (endIndex - RequiredCandles + 1);
                double rangePush3 = H(bars, endIndex, push3Idx) - L(bars, endIndex, push3Idx);
                double rangePush1 = H(bars, endIndex, push1Idx) - L(bars, endIndex, push1Idx);
                if (rangePush3 >= rangePush1) return false;

                // 5. Reversal: reazione significativa dal terzo push (mirror)
                if (C(bars, endIndex, 11) >= push3.Price - 0.5 * localATR) return false;

                _push1Price = push1.Price;
                _push3Price = push3.Price;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                // Caso B: rinfresca i campi privati
                if (!CheckPattern(bars, endIndex))
                    return new List<GhostCandle>();
                double target = currentPrice + (_push1Price - _push3Price) * 0.618;
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // --- N.27 Trap and Snap (Bull: bear trap → snap up) ---
        private class BullTrapSnapPattern : PatternBase
        {
            public BullTrapSnapPattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bull Trap Snap";
            public override int RequiredCandles => 6;
            public override int GhostCandleCount => 4;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. Consolidamento (c[0]-c[2]): range stretto < 1.0 * ATR
                double maxH = double.MinValue;
                double minL = double.MaxValue;
                for (int i = 0; i <= 2; i++)
                {
                    maxH = Math.Max(maxH, H(bars, endIndex, i));
                    minL = Math.Min(minL, L(bars, endIndex, i));
                }
                double consRange = maxH - minL;
                if (consRange >= 1.0 * localATR) return false;

                // 2. Trap (c[3]): bear, L[3] < minL, body > 0.5 * range
                if (!IsBear(bars, endIndex, 3)) return false;
                if (L(bars, endIndex, 3) >= minL) return false;
                if (Body(bars, endIndex, 3) <= 0.5 * Range(bars, endIndex, 3)) return false;

                // 3. Snap (c[4]): bull, C[4] > maxH, body[4] > body[3]
                if (!IsBull(bars, endIndex, 4)) return false;
                if (C(bars, endIndex, 4) <= maxH) return false;
                if (Body(bars, endIndex, 4) <= Body(bars, endIndex, 3)) return false;

                // 4. Follow-through (c[5]): bull, C[5] > C[4]
                if (!IsBull(bars, endIndex, 5)) return false;
                if (C(bars, endIndex, 5) <= C(bars, endIndex, 4)) return false;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                // Caso A: target = currentPrice + (C[4] - L[3])
                double target = currentPrice + (C(bars, endIndex, 4) - L(bars, endIndex, 3));
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // --- N.27 Trap and Snap (Bear: bull trap → snap down) ---
        private class BearTrapSnapPattern : PatternBase
        {
            public BearTrapSnapPattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bear Trap Snap";
            public override int RequiredCandles => 6;
            public override int GhostCandleCount => 4;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. Consolidamento (c[0]-c[2]): range stretto < 1.0 * ATR
                double maxH = double.MinValue;
                double minL = double.MaxValue;
                for (int i = 0; i <= 2; i++)
                {
                    maxH = Math.Max(maxH, H(bars, endIndex, i));
                    minL = Math.Min(minL, L(bars, endIndex, i));
                }
                double consRange = maxH - minL;
                if (consRange >= 1.0 * localATR) return false;

                // 2. Trap (c[3]): bull, H[3] > maxH, body > 0.5 * range
                if (!IsBull(bars, endIndex, 3)) return false;
                if (H(bars, endIndex, 3) <= maxH) return false;
                if (Body(bars, endIndex, 3) <= 0.5 * Range(bars, endIndex, 3)) return false;

                // 3. Snap (c[4]): bear, C[4] < minL, body[4] > body[3]
                if (!IsBear(bars, endIndex, 4)) return false;
                if (C(bars, endIndex, 4) >= minL) return false;
                if (Body(bars, endIndex, 4) <= Body(bars, endIndex, 3)) return false;

                // 4. Follow-through (c[5]): bear, C[5] < C[4]
                if (!IsBear(bars, endIndex, 5)) return false;
                if (C(bars, endIndex, 5) >= C(bars, endIndex, 4)) return false;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                // Caso A: target = currentPrice + (C[4] - H[3])
                // C[4] < consolidamento, H[3] > consolidamento → differenza negativa
                double target = currentPrice + (C(bars, endIndex, 4) - H(bars, endIndex, 3));
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // --- N.28 Double Fake (Bull: due false rotture sotto → snap sopra) ---
        private class BullDoubleFakePattern : PatternBase
        {
            public BullDoubleFakePattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bull Double Fake";
            public override int RequiredCandles => 8;
            public override int GhostCandleCount => 4;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. Range (c[0]-c[3])
                double rangeHigh = double.MinValue;
                double rangeLow = double.MaxValue;
                for (int i = 0; i <= 3; i++)
                {
                    rangeHigh = Math.Max(rangeHigh, H(bars, endIndex, i));
                    rangeLow = Math.Min(rangeLow, L(bars, endIndex, i));
                }

                // 2. Prima fake (c[4]): L[4] < rangeLow e C[4] > rangeLow
                if (L(bars, endIndex, 4) >= rangeLow) return false;
                if (C(bars, endIndex, 4) <= rangeLow) return false;

                // 3. Seconda fake: esiste j in {5, 6} dove L[j] < rangeLow e C[j] > rangeLow
                bool secondFake = false;
                for (int j = 5; j <= 6; j++)
                {
                    if (L(bars, endIndex, j) < rangeLow && C(bars, endIndex, j) > rangeLow)
                    {
                        secondFake = true;
                        break;
                    }
                }
                if (!secondFake) return false;

                // 4. Snap (c[7]): C[7] > rangeHigh
                if (C(bars, endIndex, 7) <= rangeHigh) return false;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                // Caso A: target = currentPrice + (rangeHigh - rangeLow)
                double rangeHigh = double.MinValue;
                double rangeLow = double.MaxValue;
                for (int i = 0; i <= 3; i++)
                {
                    rangeHigh = Math.Max(rangeHigh, H(bars, endIndex, i));
                    rangeLow = Math.Min(rangeLow, L(bars, endIndex, i));
                }
                double target = currentPrice + (rangeHigh - rangeLow);
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // --- N.28 Double Fake (Bear: due false rotture sopra → snap sotto) ---
        private class BearDoubleFakePattern : PatternBase
        {
            public BearDoubleFakePattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bear Double Fake";
            public override int RequiredCandles => 8;
            public override int GhostCandleCount => 4;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. Range (c[0]-c[3])
                double rangeHigh = double.MinValue;
                double rangeLow = double.MaxValue;
                for (int i = 0; i <= 3; i++)
                {
                    rangeHigh = Math.Max(rangeHigh, H(bars, endIndex, i));
                    rangeLow = Math.Min(rangeLow, L(bars, endIndex, i));
                }

                // 2. Prima fake (c[4]): H[4] > rangeHigh e C[4] < rangeHigh
                if (H(bars, endIndex, 4) <= rangeHigh) return false;
                if (C(bars, endIndex, 4) >= rangeHigh) return false;

                // 3. Seconda fake: esiste j in {5, 6} dove H[j] > rangeHigh e C[j] < rangeHigh
                bool secondFake = false;
                for (int j = 5; j <= 6; j++)
                {
                    if (H(bars, endIndex, j) > rangeHigh && C(bars, endIndex, j) < rangeHigh)
                    {
                        secondFake = true;
                        break;
                    }
                }
                if (!secondFake) return false;

                // 4. Snap (c[7]): C[7] < rangeLow
                if (C(bars, endIndex, 7) >= rangeLow) return false;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                // Caso A: target = currentPrice - (rangeHigh - rangeLow)
                double rangeHigh = double.MinValue;
                double rangeLow = double.MaxValue;
                for (int i = 0; i <= 3; i++)
                {
                    rangeHigh = Math.Max(rangeHigh, H(bars, endIndex, i));
                    rangeLow = Math.Min(rangeLow, L(bars, endIndex, i));
                }
                double target = currentPrice - (rangeHigh - rangeLow);
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.29 Bull Shark ----
        private class BullSharkPattern : PatternBase
        {
            private double _bPrice;
            private double _aPrice;

            public BullSharkPattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bull Shark";
            public override int RequiredCandles => 20;
            public override int GhostCandleCount => 5;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. FindSwings: O(low), X(high), A(low), B(high), C(low)
                var swings = FindSwings(bars, endIndex - 19, endIndex);
                int offset = swings.Count > 0 && swings[0].IsHigh ? 1 : 0;
                if (swings.Count < 5 + offset) return false;

                var sO = swings[offset];     // low
                var sX = swings[offset + 1]; // high
                var sA = swings[offset + 2]; // low
                var sB = swings[offset + 3]; // high
                var sC = swings[offset + 4]; // low

                // 2. Extension AB: AB/XA in [1.13, 1.618]
                double XA = sX.Price - sA.Price;
                if (Math.Abs(XA) < 0.1 * localATR) return false;
                double AB = sB.Price - sA.Price;
                double ratioAB_XA = AB / XA;
                if (ratioAB_XA < 1.13 || ratioAB_XA > 1.618) return false;

                // 3. Extension BC: BC/AB in [1.618, 2.24]
                if (Math.Abs(AB) < 0.1 * localATR) return false;
                double BC = sB.Price - sC.Price;
                double ratioBC_AB = BC / AB;
                if (ratioBC_AB < 1.618 || ratioBC_AB > 2.24) return false;

                // 4. Level OC: FibCheck(OC/OX, 0.886, 0.10)
                double OC = sO.Price - sC.Price;
                double OX = sX.Price - sO.Price;
                if (Math.Abs(OX) < 0.1 * localATR) return false;
                if (!FibCheck(OC / OX, 0.886, 0.10)) return false;

                // 5. Proximity: C nelle ultime 4 barre
                if (sC.Index < endIndex - 3) return false;

                _bPrice = sB.Price;
                _aPrice = sA.Price;
                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                // Caso B: refresh private fields
                if (!CheckPattern(bars, endIndex))
                    return new List<GhostCandle>();
                double target = currentPrice + (_bPrice - _aPrice) * 0.50;
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.29 Bear Shark ----
        private class BearSharkPattern : PatternBase
        {
            private double _bPrice;
            private double _aPrice;

            public BearSharkPattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bear Shark";
            public override int RequiredCandles => 20;
            public override int GhostCandleCount => 5;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. FindSwings: O(high), X(low), A(high), B(low), C(high)
                var swings = FindSwings(bars, endIndex - 19, endIndex);
                int offset = swings.Count > 0 && !swings[0].IsHigh ? 1 : 0;
                if (swings.Count < 5 + offset) return false;

                var sO = swings[offset];     // high
                var sX = swings[offset + 1]; // low
                var sA = swings[offset + 2]; // high
                var sB = swings[offset + 3]; // low
                var sC = swings[offset + 4]; // high

                // 2. Extension AB: AB/XA in [1.13, 1.618]
                double XA = sX.Price - sA.Price;
                if (Math.Abs(XA) < 0.1 * localATR) return false;
                double AB = sB.Price - sA.Price;
                double ratioAB_XA = AB / XA;
                if (ratioAB_XA < 1.13 || ratioAB_XA > 1.618) return false;

                // 3. Extension BC: BC/AB in [1.618, 2.24]
                if (Math.Abs(AB) < 0.1 * localATR) return false;
                double BC = sB.Price - sC.Price;
                double ratioBC_AB = BC / AB;
                if (ratioBC_AB < 1.618 || ratioBC_AB > 2.24) return false;

                // 4. Level OC: FibCheck(OC/OX, 0.886, 0.10)
                double OC = sO.Price - sC.Price;
                double OX = sX.Price - sO.Price;
                if (Math.Abs(OX) < 0.1 * localATR) return false;
                if (!FibCheck(OC / OX, 0.886, 0.10)) return false;

                // 5. Proximity: C nelle ultime 4 barre
                if (sC.Index < endIndex - 3) return false;

                _bPrice = sB.Price;
                _aPrice = sA.Price;
                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                // Caso B: refresh private fields
                if (!CheckPattern(bars, endIndex))
                    return new List<GhostCandle>();
                double target = currentPrice + (_bPrice - _aPrice) * 0.50;
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.30 Bull Cypher ----
        private class BullCypherPattern : PatternBase
        {
            private double _aPrice;
            private double _xPrice;

            public BullCypherPattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bull Cypher";
            public override int RequiredCandles => 20;
            public override int GhostCandleCount => 5;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. FindSwings: X(low), A(high), B(low), C(high), D(low)
                var swings = FindSwings(bars, endIndex - 19, endIndex);
                int offset = swings.Count > 0 && swings[0].IsHigh ? 1 : 0;
                if (swings.Count < 5 + offset) return false;

                var sX = swings[offset];     // low
                var sA = swings[offset + 1]; // high
                var sB = swings[offset + 2]; // low
                var sC = swings[offset + 3]; // high
                var sD = swings[offset + 4]; // low

                // 2. Retracement AB: AB/XA in [0.382, 0.618]
                double XA = sA.Price - sX.Price;
                if (Math.Abs(XA) < 0.1 * localATR) return false;
                double AB = sA.Price - sB.Price;
                double ratioAB_XA = AB / XA;
                if (ratioAB_XA < 0.382 || ratioAB_XA > 0.618) return false;

                // 3. Extension XC: C supera A, XC/XA in [1.13, 1.414]
                if (sC.Price <= sA.Price) return false;
                double XC = sC.Price - sX.Price;
                if (Math.Abs(XC) < 0.1 * localATR) return false;
                double ratioXC_XA = XC / XA;
                if (ratioXC_XA < 1.13 || ratioXC_XA > 1.414) return false;

                // 4. Retracement CD: FibCheck(CD/XC, 0.786, 0.05)
                double CD = sC.Price - sD.Price;
                if (!FibCheck(CD / XC, 0.786, 0.05)) return false;

                // 5. Proximity: D nelle ultime 4 barre
                if (sD.Index < endIndex - 3) return false;

                _aPrice = sA.Price;
                _xPrice = sX.Price;
                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                // Caso B: refresh private fields
                if (!CheckPattern(bars, endIndex))
                    return new List<GhostCandle>();
                double target = currentPrice + (_aPrice - _xPrice) * 0.382;
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.30 Bear Cypher ----
        private class BearCypherPattern : PatternBase
        {
            private double _aPrice;
            private double _xPrice;

            public BearCypherPattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bear Cypher";
            public override int RequiredCandles => 20;
            public override int GhostCandleCount => 5;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. FindSwings: X(high), A(low), B(high), C(low), D(high)
                var swings = FindSwings(bars, endIndex - 19, endIndex);
                int offset = swings.Count > 0 && !swings[0].IsHigh ? 1 : 0;
                if (swings.Count < 5 + offset) return false;

                var sX = swings[offset];     // high
                var sA = swings[offset + 1]; // low
                var sB = swings[offset + 2]; // high
                var sC = swings[offset + 3]; // low
                var sD = swings[offset + 4]; // high

                // 2. Retracement AB: AB/XA in [0.382, 0.618]
                double XA = sA.Price - sX.Price;
                if (Math.Abs(XA) < 0.1 * localATR) return false;
                double AB = sA.Price - sB.Price;
                double ratioAB_XA = AB / XA;
                if (ratioAB_XA < 0.382 || ratioAB_XA > 0.618) return false;

                // 3. Extension XC: C scende sotto A, XC/XA in [1.13, 1.414]
                if (sC.Price >= sA.Price) return false;
                double XC = sC.Price - sX.Price;
                if (Math.Abs(XC) < 0.1 * localATR) return false;
                double ratioXC_XA = XC / XA;
                if (ratioXC_XA < 1.13 || ratioXC_XA > 1.414) return false;

                // 4. Retracement CD: FibCheck(CD/XC, 0.786, 0.05)
                double CD = sC.Price - sD.Price;
                if (!FibCheck(CD / XC, 0.786, 0.05)) return false;

                // 5. Proximity: D nelle ultime 4 barre
                if (sD.Index < endIndex - 3) return false;

                _aPrice = sA.Price;
                _xPrice = sX.Price;
                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                // Caso B: refresh private fields
                if (!CheckPattern(bars, endIndex))
                    return new List<GhostCandle>();
                double target = currentPrice + (_aPrice - _xPrice) * 0.382;
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.31 Bull 5-0 ----
        private class Bull50Pattern : PatternBase
        {
            private double _bPrice;
            private double _cPrice;

            public Bull50Pattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bull 5-0";
            public override int RequiredCandles => 20;
            public override int GhostCandleCount => 5;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. FindSwings: O(high), A(low), B(high), C(low) — use last 4
                var swings = FindSwings(bars, endIndex - 19, endIndex);
                if (swings.Count < 4) return false;
                int si = swings.Count - 4;
                var sO = swings[si];
                if (!sO.IsHigh) return false; // O must be swing high
                var sA = swings[si + 1]; // low
                var sB = swings[si + 2]; // high
                var sC = swings[si + 3]; // low

                // B supera O
                if (sB.Price <= sO.Price) return false;

                // Vincolo temporale: C recente
                if (endIndex - sC.Index > 8) return false;

                // 2. Guard divisione per zero
                double OA = sO.Price - sA.Price;
                double AB = sB.Price - sA.Price;
                double BC = sB.Price - sC.Price;
                if (Math.Abs(OA) < 0.1 * localATR) return false;
                if (Math.Abs(AB) < 0.1 * localATR) return false;
                if (Math.Abs(BC) < 0.1 * localATR) return false;

                // 3. Extension AB: AB/OA in [1.13, 1.618]
                double ratioAB = AB / OA;
                if (ratioAB < 1.13 || ratioAB > 1.618) return false;

                // 4. Extension BC: BC/AB in [1.618, 2.24]
                double ratioBC = BC / AB;
                if (ratioBC < 1.618 || ratioBC > 2.24) return false;

                // 5. Retracement CD: D.price = C[19], CD/BC ≈ 0.50
                double dPrice = C(bars, endIndex, 19);
                double CD = dPrice - sC.Price;
                if (!FibCheck(CD / BC, 0.50, 0.10)) return false;

                _bPrice = sB.Price;
                _cPrice = sC.Price;
                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                // Caso B: refresh private fields
                if (!CheckPattern(bars, endIndex))
                    return new List<GhostCandle>();
                double target = currentPrice + (_bPrice - _cPrice) * 0.382;
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.31 Bear 5-0 ----
        private class Bear50Pattern : PatternBase
        {
            private double _bPrice;
            private double _cPrice;

            public Bear50Pattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bear 5-0";
            public override int RequiredCandles => 20;
            public override int GhostCandleCount => 5;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. FindSwings: O(low), A(high), B(low), C(high) — use last 4
                var swings = FindSwings(bars, endIndex - 19, endIndex);
                if (swings.Count < 4) return false;
                int si = swings.Count - 4;
                var sO = swings[si];
                if (sO.IsHigh) return false; // O must be swing low for bear
                var sA = swings[si + 1]; // high
                var sB = swings[si + 2]; // low
                var sC = swings[si + 3]; // high

                // B sotto O
                if (sB.Price >= sO.Price) return false;

                // Vincolo temporale: C recente
                if (endIndex - sC.Index > 8) return false;

                // 2. Guard divisione per zero
                double OA = sO.Price - sA.Price;
                double AB = sB.Price - sA.Price;
                double BC = sB.Price - sC.Price;
                if (Math.Abs(OA) < 0.1 * localATR) return false;
                if (Math.Abs(AB) < 0.1 * localATR) return false;
                if (Math.Abs(BC) < 0.1 * localATR) return false;

                // 3. Extension AB: AB/OA in [1.13, 1.618]
                double ratioAB = AB / OA;
                if (ratioAB < 1.13 || ratioAB > 1.618) return false;

                // 4. Extension BC: BC/AB in [1.618, 2.24]
                double ratioBC = BC / AB;
                if (ratioBC < 1.618 || ratioBC > 2.24) return false;

                // 5. Retracement CD: D.price = C[19], CD/BC ≈ 0.50
                double dPrice = C(bars, endIndex, 19);
                double CD = dPrice - sC.Price;
                if (!FibCheck(CD / BC, 0.50, 0.10)) return false;

                _bPrice = sB.Price;
                _cPrice = sC.Price;
                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                // Caso B: refresh private fields
                if (!CheckPattern(bars, endIndex))
                    return new List<GhostCandle>();
                double target = currentPrice + (_bPrice - _cPrice) * 0.382;
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.32 Bull Alt Bat ----
        private class BullAltBatPattern : PatternBase
        {
            private double _aPrice;
            private double _xPrice;

            public BullAltBatPattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bull Alt Bat";
            public override int RequiredCandles => 20;
            public override int GhostCandleCount => 5;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. FindSwings: X(low), A(high), B(low), C(high), D(low) — use last 5
                var swings = FindSwings(bars, endIndex - 19, endIndex);
                if (swings.Count < 5) return false;
                int si = swings.Count - 5;
                var sX = swings[si];
                if (sX.IsHigh) return false; // X must be swing low for bull
                var sA = swings[si + 1]; // high
                var sB = swings[si + 2]; // low
                var sC = swings[si + 3]; // high
                var sD = swings[si + 4]; // low

                // Vincolo temporale: D recente
                if (endIndex - sD.Index > 3) return false;

                // 2. Guard divisione per zero
                double XA = sA.Price - sX.Price;
                double AB = sA.Price - sB.Price;
                if (Math.Abs(XA) < 0.1 * localATR) return false;
                if (Math.Abs(AB) < 0.1 * localATR) return false;

                // 3. Retracement AB: FibCheck(AB/XA, 0.382, 0.07)
                if (!FibCheck(AB / XA, 0.382, 0.07)) return false;

                // 4. Retracement BC: BC/AB in [0.382, 0.886]
                double BC = sC.Price - sB.Price;
                double ratioBC = BC / AB;
                if (ratioBC < 0.382 || ratioBC > 0.886) return false;

                // 5. Extension AD: FibCheck(AD/XA, 1.13, 0.05)
                double AD = sA.Price - sD.Price;
                if (!FibCheck(AD / XA, 1.13, 0.05)) return false;

                _aPrice = sA.Price;
                _xPrice = sX.Price;
                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                // Caso B: refresh private fields
                if (!CheckPattern(bars, endIndex))
                    return new List<GhostCandle>();
                double target = currentPrice + (_aPrice - _xPrice) * 0.382;
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.32 Bear Alt Bat ----
        private class BearAltBatPattern : PatternBase
        {
            private double _aPrice;
            private double _xPrice;

            public BearAltBatPattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bear Alt Bat";
            public override int RequiredCandles => 20;
            public override int GhostCandleCount => 5;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. FindSwings: X(high), A(low), B(high), C(low), D(high) — use last 5
                var swings = FindSwings(bars, endIndex - 19, endIndex);
                if (swings.Count < 5) return false;
                int si = swings.Count - 5;
                var sX = swings[si];
                if (!sX.IsHigh) return false; // X must be swing high for bear
                var sA = swings[si + 1]; // low
                var sB = swings[si + 2]; // high
                var sC = swings[si + 3]; // low
                var sD = swings[si + 4]; // high

                // Vincolo temporale: D recente
                if (endIndex - sD.Index > 3) return false;

                // 2. Guard divisione per zero
                double XA = sA.Price - sX.Price;
                double AB = sA.Price - sB.Price;
                if (Math.Abs(XA) < 0.1 * localATR) return false;
                if (Math.Abs(AB) < 0.1 * localATR) return false;

                // 3. Retracement AB: FibCheck(AB/XA, 0.382, 0.07)
                if (!FibCheck(AB / XA, 0.382, 0.07)) return false;

                // 4. Retracement BC: BC/AB in [0.382, 0.886]
                double BC = sC.Price - sB.Price;
                double ratioBC = BC / AB;
                if (ratioBC < 0.382 || ratioBC > 0.886) return false;

                // 5. Extension AD: FibCheck(AD/XA, 1.13, 0.05)
                double AD = sA.Price - sD.Price;
                if (!FibCheck(AD / XA, 1.13, 0.05)) return false;

                _aPrice = sA.Price;
                _xPrice = sX.Price;
                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                // Caso B: refresh private fields
                if (!CheckPattern(bars, endIndex))
                    return new List<GhostCandle>();
                double target = currentPrice + (_aPrice - _xPrice) * 0.382;
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.33 Bull Ross Hook ----
        private class BullRossHookPattern : PatternBase
        {
            public BullRossHookPattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bull Ross Hook";
            public override int RequiredCandles => 10;
            public override int GhostCandleCount => 5;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. Punto 1 (low): min(L[0]..L[3])
                double punto1 = double.MaxValue;
                for (int i = 0; i <= 3; i++)
                    punto1 = Math.Min(punto1, L(bars, endIndex, i));

                // 2. Punto 2 (reaction high): max(H[2]..H[5])
                double punto2 = double.MinValue;
                for (int i = 2; i <= 5; i++)
                    punto2 = Math.Max(punto2, H(bars, endIndex, i));
                // Deve essere significativo
                if (punto2 <= punto1 + localATR) return false;

                // 3. Punto 3 (higher low): min(L[4]..L[5])
                double punto3 = Math.Min(L(bars, endIndex, 4), L(bars, endIndex, 5));
                if (punto3 <= punto1) return false;
                if (punto3 >= punto2) return false;

                // 4. Attraversamento: max(H[5]..H[7]) > punto2
                double maxH57 = double.MinValue;
                for (int i = 5; i <= 7; i++)
                    maxH57 = Math.Max(maxH57, H(bars, endIndex, i));
                if (maxH57 <= punto2) return false;

                // 5. Hook correction (c[6]-c[8]): min(L[6]..L[8]) > punto3
                double minL68 = double.MaxValue;
                for (int i = 6; i <= 8; i++)
                    minL68 = Math.Min(minL68, L(bars, endIndex, i));
                if (minL68 <= punto3) return false;

                // 6. Break hook: C[9] > max(H[6]..H[8])
                double maxH68 = double.MinValue;
                for (int i = 6; i <= 8; i++)
                    maxH68 = Math.Max(maxH68, H(bars, endIndex, i));
                if (C(bars, endIndex, 9) <= maxH68) return false;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                // Caso A: ricalcola punto1/punto2 direttamente
                double localATR = LocalATR(endIndex);

                double punto1 = double.MaxValue;
                for (int i = 0; i <= 3; i++)
                    punto1 = Math.Min(punto1, L(bars, endIndex, i));

                double punto2 = double.MinValue;
                for (int i = 2; i <= 5; i++)
                    punto2 = Math.Max(punto2, H(bars, endIndex, i));

                double target = currentPrice + Math.Min(punto2 - punto1, 2.0 * localATR);
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.33 Bear Ross Hook (mirror) ----
        private class BearRossHookPattern : PatternBase
        {
            public BearRossHookPattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bear Ross Hook";
            public override int RequiredCandles => 10;
            public override int GhostCandleCount => 5;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. Punto 1 (high): max(H[0]..H[3])
                double punto1 = double.MinValue;
                for (int i = 0; i <= 3; i++)
                    punto1 = Math.Max(punto1, H(bars, endIndex, i));

                // 2. Punto 2 (reaction low): min(L[2]..L[5])
                double punto2 = double.MaxValue;
                for (int i = 2; i <= 5; i++)
                    punto2 = Math.Min(punto2, L(bars, endIndex, i));
                // Deve essere significativo
                if (punto2 >= punto1 - localATR) return false;

                // 3. Punto 3 (lower high): max(H[4]..H[5])
                double punto3 = Math.Max(H(bars, endIndex, 4), H(bars, endIndex, 5));
                if (punto3 >= punto1) return false;
                if (punto3 <= punto2) return false;

                // 4. Attraversamento: min(L[5]..L[7]) < punto2
                double minL57 = double.MaxValue;
                for (int i = 5; i <= 7; i++)
                    minL57 = Math.Min(minL57, L(bars, endIndex, i));
                if (minL57 >= punto2) return false;

                // 5. Hook correction (c[6]-c[8]): max(H[6]..H[8]) < punto3
                double maxH68 = double.MinValue;
                for (int i = 6; i <= 8; i++)
                    maxH68 = Math.Max(maxH68, H(bars, endIndex, i));
                if (maxH68 >= punto3) return false;

                // 6. Break hook: C[9] < min(L[6]..L[8])
                double minL68 = double.MaxValue;
                for (int i = 6; i <= 8; i++)
                    minL68 = Math.Min(minL68, L(bars, endIndex, i));
                if (C(bars, endIndex, 9) >= minL68) return false;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                // Caso A: ricalcola punto1/punto2 direttamente
                double localATR = LocalATR(endIndex);

                double punto1 = double.MinValue;
                for (int i = 0; i <= 3; i++)
                    punto1 = Math.Max(punto1, H(bars, endIndex, i));

                double punto2 = double.MaxValue;
                for (int i = 2; i <= 5; i++)
                    punto2 = Math.Min(punto2, L(bars, endIndex, i));

                // punto2 - punto1 e' negativo, cap con max(..., -2.0 * localATR)
                double target = currentPrice + Math.Max(punto2 - punto1, -2.0 * localATR);
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.34 Bull 2B Rev ----
        private class Bull2BPattern : PatternBase
        {
            private double _swingHigh;
            private double _swingRange;

            public Bull2BPattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bull 2B Rev";
            public override int RequiredCandles => 8;
            public override int GhostCandleCount => 4;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. Swing low precedente e swing high
                double prevLow = double.MaxValue;
                for (int i = 0; i <= 2; i++)
                    prevLow = Math.Min(prevLow, L(bars, endIndex, i));

                double swingHigh = double.MinValue;
                for (int i = 0; i <= 2; i++)
                    swingHigh = Math.Max(swingHigh, H(bars, endIndex, i));

                // 2. Nuovo test (c[4]-c[5]): newLow <= prevLow + 0.05 * localATR
                double newLow = Math.Min(L(bars, endIndex, 4), L(bars, endIndex, 5));
                if (newLow > prevLow + 0.05 * localATR) return false;

                // 3. Failure: C[6] > max(H[4], H[5])
                double maxH45 = Math.Max(H(bars, endIndex, 4), H(bars, endIndex, 5));
                if (C(bars, endIndex, 6) <= maxH45) return false;

                // 4. Continuazione: C[7] > C[6]
                if (C(bars, endIndex, 7) <= C(bars, endIndex, 6)) return false;

                // Salva campi privati per GenerateGhosts
                _swingHigh = swingHigh;
                _swingRange = swingHigh - prevLow;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                // Caso B: rinfresca campi privati; se il pattern non e piu valido, nessuna ghost
                if (!CheckPattern(bars, endIndex))
                    return new List<GhostCandle>();
                double target = currentPrice + _swingRange * 0.618;
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.34 Bear 2B Rev (bear esplicito — condizione 2 bidirezionale) ----
        private class Bear2BPattern : PatternBase
        {
            private double _swingHigh;
            private double _swingRange;

            public Bear2BPattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bear 2B Rev";
            public override int RequiredCandles => 8;
            public override int GhostCandleCount => 4;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. Swing high precedente
                double prevHigh = double.MinValue;
                for (int i = 0; i <= 2; i++)
                    prevHigh = Math.Max(prevHigh, H(bars, endIndex, i));

                double prevLow = double.MaxValue;
                for (int i = 0; i <= 2; i++)
                    prevLow = Math.Min(prevLow, L(bars, endIndex, i));

                // 2. Nuovo test (c[4]-c[5]) — BEAR ESPLICITO: tolleranza bidirezionale
                double newHigh = Math.Max(H(bars, endIndex, 4), H(bars, endIndex, 5));
                if (newHigh < prevHigh - 0.05 * localATR) return false;
                if (newHigh > prevHigh + 0.05 * localATR) return false;

                // 3. Failure: C[6] < min(L[4], L[5])
                double minL45 = Math.Min(L(bars, endIndex, 4), L(bars, endIndex, 5));
                if (C(bars, endIndex, 6) >= minL45) return false;

                // 4. Continuazione: C[7] < C[6]
                if (C(bars, endIndex, 7) >= C(bars, endIndex, 6)) return false;

                // Salva campi privati — swingRange usa SEMPRE max(H) - min(L), non mirror
                _swingHigh = prevHigh;
                _swingRange = prevHigh - prevLow;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                // Caso B: rinfresca campi privati; se il pattern non e piu valido, nessuna ghost
                if (!CheckPattern(bars, endIndex))
                    return new List<GhostCandle>();
                double target = currentPrice - _swingRange * 0.618;
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.35 Bull Turtle Soup ----
        private class BullTurtleSoupPattern : PatternBase
        {
            public BullTurtleSoupPattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bull Turtle Soup";
            public override int RequiredCandles => 6;
            public override int GhostCandleCount => 4;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. Livello chiave: range delle prime 4 candele
                double keyLow = double.MaxValue;
                for (int i = 0; i <= 3; i++)
                    keyLow = Math.Min(keyLow, L(bars, endIndex, i));

                double keyHigh = double.MinValue;
                for (int i = 0; i <= 3; i++)
                    keyHigh = Math.Max(keyHigh, H(bars, endIndex, i));

                if (keyHigh - keyLow <= 0.3 * localATR) return false;

                // 2. Falsa rottura (c[4]): penetra sotto, chiude sopra, penetrazione significativa
                if (L(bars, endIndex, 4) >= keyLow) return false;
                if (C(bars, endIndex, 4) <= keyLow) return false;
                if (keyLow - L(bars, endIndex, 4) <= 0.1 * localATR) return false;

                // 3. Reazione (c[5]): chiude sopra il high della falsa rottura
                if (C(bars, endIndex, 5) <= H(bars, endIndex, 4)) return false;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                // Caso A: target calcolabile direttamente
                double keyLow = double.MaxValue;
                for (int i = 0; i <= 3; i++)
                    keyLow = Math.Min(keyLow, L(bars, endIndex, i));

                double keyHigh = double.MinValue;
                for (int i = 0; i <= 3; i++)
                    keyHigh = Math.Max(keyHigh, H(bars, endIndex, i));

                double keyRange = keyHigh - keyLow;
                double target = currentPrice + keyRange * 0.5;
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.35 Bear Turtle Soup (bear esplicito — condizione 2 penetrazione invertita) ----
        private class BearTurtleSoupPattern : PatternBase
        {
            public BearTurtleSoupPattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bear Turtle Soup";
            public override int RequiredCandles => 6;
            public override int GhostCandleCount => 4;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. Livello chiave: range delle prime 4 candele
                double keyLow = double.MaxValue;
                for (int i = 0; i <= 3; i++)
                    keyLow = Math.Min(keyLow, L(bars, endIndex, i));

                double keyHigh = double.MinValue;
                for (int i = 0; i <= 3; i++)
                    keyHigh = Math.Max(keyHigh, H(bars, endIndex, i));

                if (keyHigh - keyLow <= 0.3 * localATR) return false;

                // 2. Falsa rottura BEAR (ESPLICITO): penetra sopra, chiude sotto, penetrazione significativa
                if (H(bars, endIndex, 4) <= keyHigh) return false;
                if (C(bars, endIndex, 4) >= keyHigh) return false;
                if (H(bars, endIndex, 4) - keyHigh <= 0.1 * localATR) return false;

                // 3. Reazione bear: chiude sotto il low della falsa rottura
                if (C(bars, endIndex, 5) >= L(bars, endIndex, 4)) return false;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                // Caso A: keyRange = keyHigh - keyLow (invariante, NON mirror)
                double keyLow = double.MaxValue;
                for (int i = 0; i <= 3; i++)
                    keyLow = Math.Min(keyLow, L(bars, endIndex, i));

                double keyHigh = double.MinValue;
                for (int i = 0; i <= 3; i++)
                    keyHigh = Math.Max(keyHigh, H(bars, endIndex, i));

                double keyRange = keyHigh - keyLow;
                double target = currentPrice - keyRange * 0.5;
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.36 Bull Breaker Block Retest ----
        private class BullBreakerPattern : PatternBase
        {
            public BullBreakerPattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bull Breaker";
            public override int RequiredCandles => 8;
            public override int GhostCandleCount => 4;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. Order block bearish (c[0]): candela istituzionale bearish con body significativo
                if (!IsBear(bars, endIndex, 0)) return false;
                if (Body(bars, endIndex, 0) <= 0.5 * Range(bars, endIndex, 0)) return false;

                double obHigh = H(bars, endIndex, 0);
                double obLow = L(bars, endIndex, 0);

                // Guard: range dell'OB non nullo
                if (obHigh - obLow < 0.1 * localATR) return false;

                // 2. Rottura rialzista: prezzo interagisce con zona OB, poi rompe sopra
                double minL13 = double.MaxValue;
                for (int i = 1; i <= 3; i++)
                    minL13 = Math.Min(minL13, L(bars, endIndex, i));

                if (minL13 > obHigh) return false;
                if (C(bars, endIndex, 4) <= obHigh) return false;

                // 3. Retest (c[5]-c[6]): prezzo torna nella zona dall'alto
                double minL56 = Math.Min(L(bars, endIndex, 5), L(bars, endIndex, 6));
                if (minL56 < obLow) return false;
                if (minL56 > obHigh) return false;

                // 4. Rejection (c[7]): rimbalzo dalla zona breaker
                if (C(bars, endIndex, 7) <= obHigh) return false;
                if (L(bars, endIndex, 7) <= obLow) return false;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                // Caso A: target da dimensione dell'order block
                double obHigh = H(bars, endIndex, 0);
                double obLow = L(bars, endIndex, 0);
                double target = currentPrice + (obHigh - obLow) * 1.5;
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.36 Bear Breaker Block Retest (mirror — OB bullish rotto al ribasso) ----
        private class BearBreakerPattern : PatternBase
        {
            public BearBreakerPattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bear Breaker";
            public override int RequiredCandles => 8;
            public override int GhostCandleCount => 4;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. Order block bullish (c[0]): candela istituzionale bullish con body significativo
                if (!IsBull(bars, endIndex, 0)) return false;
                if (Body(bars, endIndex, 0) <= 0.5 * Range(bars, endIndex, 0)) return false;

                double obHigh = H(bars, endIndex, 0);
                double obLow = L(bars, endIndex, 0);

                // Guard: range dell'OB non nullo
                if (obHigh - obLow < 0.1 * localATR) return false;

                // 2. Rottura ribassista: prezzo interagisce con zona OB, poi rompe sotto
                double maxH13 = double.MinValue;
                for (int i = 1; i <= 3; i++)
                    maxH13 = Math.Max(maxH13, H(bars, endIndex, i));

                if (maxH13 < obLow) return false;
                if (C(bars, endIndex, 4) >= obLow) return false;

                // 3. Retest (c[5]-c[6]): prezzo torna nella zona dal basso
                double maxH56 = Math.Max(H(bars, endIndex, 5), H(bars, endIndex, 6));
                if (maxH56 > obHigh) return false;
                if (maxH56 < obLow) return false;

                // 4. Rejection (c[7]): respinto dalla zona breaker verso il basso
                if (C(bars, endIndex, 7) >= obLow) return false;
                if (H(bars, endIndex, 7) >= obHigh) return false;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                // Caso A: target da dimensione dell'order block (obHigh - obLow sempre positivo)
                double obHigh = H(bars, endIndex, 0);
                double obLow = L(bars, endIndex, 0);
                double target = currentPrice - (obHigh - obLow) * 1.5;
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // --- N.37 FVG Fill + Rejection (Bull) ---
        private class BullFVGPattern : PatternBase
        {
            public BullFVGPattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bull FVG Reject";
            public override int RequiredCandles => 6;
            public override int GhostCandleCount => 4;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. FVG ribassista (c[0]–c[2]): L[0] > H[2], gap significativo, candela centrale bearish
                if (L(bars, endIndex, 0) <= H(bars, endIndex, 2)) return false;
                if (L(bars, endIndex, 0) - H(bars, endIndex, 2) <= 0.2 * localATR) return false;
                if (!IsBear(bars, endIndex, 1)) return false;

                // 2. Fill (c[3]–c[4]): almeno una candela penetra nel gap dal basso
                if (H(bars, endIndex, 3) <= H(bars, endIndex, 2) && H(bars, endIndex, 4) <= H(bars, endIndex, 2)) return false;

                // 3. Rejection (c[5]): chiude sopra il top della zona FVG, body significativo
                if (C(bars, endIndex, 5) <= L(bars, endIndex, 0)) return false;
                if (Body(bars, endIndex, 5) <= 0.5 * Range(bars, endIndex, 5)) return false;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                // Caso A: target = currentPrice + (H[1] - L[1]) — sempre positivo
                double target = currentPrice + (H(bars, endIndex, 1) - L(bars, endIndex, 1));
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // --- N.37 FVG Fill + Rejection (Bear — Mirror) ---
        private class BearFVGPattern : PatternBase
        {
            public BearFVGPattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bear FVG Reject";
            public override int RequiredCandles => 6;
            public override int GhostCandleCount => 4;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. FVG rialzista (c[0]–c[2]): H[0] < L[2], gap significativo, candela centrale bullish
                if (H(bars, endIndex, 0) >= L(bars, endIndex, 2)) return false;
                if (L(bars, endIndex, 2) - H(bars, endIndex, 0) <= 0.2 * localATR) return false;
                if (!IsBull(bars, endIndex, 1)) return false;

                // 2. Fill dall'alto (c[3]–c[4]): almeno una candela penetra nel gap dall'alto
                if (L(bars, endIndex, 3) >= L(bars, endIndex, 2) && L(bars, endIndex, 4) >= L(bars, endIndex, 2)) return false;

                // 3. Rejection bearish (c[5]): chiude sotto il bottom della zona FVG, body significativo
                if (C(bars, endIndex, 5) >= H(bars, endIndex, 0)) return false;
                if (Body(bars, endIndex, 5) <= 0.5 * Range(bars, endIndex, 5)) return false;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                // Mirror target: currentPrice + (L[1] - H[1]) — sempre negativo
                double target = currentPrice + (L(bars, endIndex, 1) - H(bars, endIndex, 1));
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // --- N.38 Two-Legged Pullback (Bull) ---
        private class BullTwoLegPBPattern : PatternBase
        {
            public BullTwoLegPBPattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bull Two-Leg PB";
            public override int RequiredCandles => 10;
            public override int GhostCandleCount => 5;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. Impulso up (c[0]–c[1])
                if (C(bars, endIndex, 1) <= O(bars, endIndex, 0)) return false;
                if (Math.Abs(C(bars, endIndex, 1) - O(bars, endIndex, 0)) <= 1.0 * localATR) return false;
                if (!IsBull(bars, endIndex, 0) && !IsBull(bars, endIndex, 1)) return false;

                // 2. Leg 1 down (c[2]–c[4]): pullback, non rompe l'origine
                if (L(bars, endIndex, 4) >= C(bars, endIndex, 1)) return false;
                double minL24 = double.MaxValue;
                for (int i = 2; i <= 4; i++)
                    minL24 = Math.Min(minL24, L(bars, endIndex, i));
                if (minL24 <= O(bars, endIndex, 0)) return false;

                // 3. Mini bounce (c[5])
                if (!IsBull(bars, endIndex, 5) && C(bars, endIndex, 5) <= C(bars, endIndex, 4)) return false;

                // 4. Leg 2 down (c[6]–c[7]): scende sotto il bounce, non sfonda troppo sotto leg 1
                if (L(bars, endIndex, 7) >= L(bars, endIndex, 5)) return false;
                if (L(bars, endIndex, 7) <= minL24 - 0.3 * localATR) return false;

                // 5. Reversal (c[8]–c[9]): chiude sopra il high del mini bounce
                if (C(bars, endIndex, 9) <= H(bars, endIndex, 5)) return false;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                // Caso A: formula identica bull/bear — NON applicare mirror O↔C
                // Bull: C[1] > O[0] → positivo → target sopra currentPrice
                double target = currentPrice + (C(bars, endIndex, 1) - O(bars, endIndex, 0)) * 0.618;
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // --- N.38 Two-Legged Pullback (Bear — Mirror eccetto condizione 4) ---
        private class BearTwoLegPBPattern : PatternBase
        {
            public BearTwoLegPBPattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bear Two-Leg PB";
            public override int RequiredCandles => 10;
            public override int GhostCandleCount => 5;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. Impulso down (c[0]–c[1])
                if (C(bars, endIndex, 1) >= O(bars, endIndex, 0)) return false;
                if (Math.Abs(C(bars, endIndex, 1) - O(bars, endIndex, 0)) <= 1.0 * localATR) return false;
                if (!IsBear(bars, endIndex, 0) && !IsBear(bars, endIndex, 1)) return false;

                // 2. Leg 1 up (c[2]–c[4]): pullback, non rompe l'origine
                if (H(bars, endIndex, 4) <= C(bars, endIndex, 1)) return false;
                double maxH24 = double.MinValue;
                for (int i = 2; i <= 4; i++)
                    maxH24 = Math.Max(maxH24, H(bars, endIndex, i));
                if (maxH24 >= O(bars, endIndex, 0)) return false;

                // 3. Mini bounce (c[5])
                if (!IsBear(bars, endIndex, 5) && C(bars, endIndex, 5) >= C(bars, endIndex, 4)) return false;

                // 4. Leg 2 up (c[6]–c[7]): ESPLICITO — upper bound, non il mirror meccanico
                if (H(bars, endIndex, 7) <= H(bars, endIndex, 5)) return false;
                if (H(bars, endIndex, 7) >= maxH24 + 0.3 * localATR) return false;

                // 5. Reversal (c[8]–c[9]): chiude sotto il low del mini bounce
                if (C(bars, endIndex, 9) >= L(bars, endIndex, 5)) return false;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                // Formula IDENTICA al bull — NON applicare mirror O↔C
                // Bear: C[1] < O[0] → negativo → target sotto currentPrice
                double target = currentPrice + (C(bars, endIndex, 1) - O(bars, endIndex, 0)) * 0.618;
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.39 Bull Bump Run ----
        private class BullBumpRunPattern : PatternBase
        {
            public BullBumpRunPattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bull Bump Run";
            public override int RequiredCandles => 16;
            public override int GhostCandleCount => 5;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. Lead-in (c[0]–c[5]): trend ribassista moderato
                double trendSlope = (C(bars, endIndex, 5) - C(bars, endIndex, 0)) / 5.0;
                if (trendSlope >= 0) return false;
                if (Math.Abs(trendSlope) >= 0.5 * localATR) return false;

                // 2. Bump (c[6]–c[11]): accelerazione ribassista
                double bumpSlope = (C(bars, endIndex, 11) - C(bars, endIndex, 6)) / 5.0;
                if (bumpSlope >= 0) return false;
                if (Math.Abs(bumpSlope) <= Math.Abs(trendSlope) * 1.5) return false;

                // 3. Bump depth: min(L[6]..L[11]) < C[5] - 1.5 * localATR
                double minLow = double.MaxValue;
                for (int i = 6; i <= 11; i++)
                    minLow = Math.Min(minLow, L(bars, endIndex, i));
                if (minLow >= C(bars, endIndex, 5) - 1.5 * localATR) return false;

                // 4. Run (c[12]–c[15]): C[15] > C[11]
                if (C(bars, endIndex, 15) <= C(bars, endIndex, 11)) return false;

                // 5. Break trendline: C[15] > leadInLevel
                double leadInLevel = C(bars, endIndex, 0) + trendSlope * 15.0;
                if (C(bars, endIndex, 15) <= leadInLevel) return false;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                double localATR = LocalATR(endIndex);
                double trendSlope = (C(bars, endIndex, 5) - C(bars, endIndex, 0)) / 5.0;
                // Trendline al centro del bump (pos. 8), non estrapolata fino a pos. 15:
                // cosi height = trendlineAtBump - minLow e sempre >= 0 per bump validi.
                double trendlineAtBump = C(bars, endIndex, 0) + trendSlope * 8.0;
                double minLow = double.MaxValue;
                for (int i = 6; i <= 11; i++)
                    minLow = Math.Min(minLow, L(bars, endIndex, i));
                double height = trendlineAtBump - minLow;
                double cappedHeight = Math.Min(height, 2.5 * localATR);
                double target = currentPrice + cappedHeight;
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ---- N.39 Bear Bump Run ----
        private class BearBumpRunPattern : PatternBase
        {
            public BearBumpRunPattern(VITPredittore_v2_2 ind) : base(ind) { }
            public override string Name => "Bear Bump Run";
            public override int RequiredCandles => 16;
            public override int GhostCandleCount => 5;

            public override bool CheckPattern(Bars bars, int endIndex)
            {
                if (endIndex < RequiredCandles - 1) return false;
                double localATR = LocalATR(endIndex);
                if (localATR <= 0) return false;

                // 1. Lead-in (c[0]–c[5]): trend rialzista moderato
                double trendSlope = (C(bars, endIndex, 5) - C(bars, endIndex, 0)) / 5.0;
                if (trendSlope <= 0) return false;
                if (Math.Abs(trendSlope) >= 0.5 * localATR) return false;

                // 2. Bump (c[6]–c[11]): accelerazione rialzista
                double bumpSlope = (C(bars, endIndex, 11) - C(bars, endIndex, 6)) / 5.0;
                if (bumpSlope <= 0) return false;
                if (Math.Abs(bumpSlope) <= Math.Abs(trendSlope) * 1.5) return false;

                // 3. Bump depth: max(H[6]..H[11]) > C[5] + 1.5 * localATR
                double maxHigh = double.MinValue;
                for (int i = 6; i <= 11; i++)
                    maxHigh = Math.Max(maxHigh, H(bars, endIndex, i));
                if (maxHigh <= C(bars, endIndex, 5) + 1.5 * localATR) return false;

                // 4. Run (c[12]–c[15]): C[15] < C[11]
                if (C(bars, endIndex, 15) >= C(bars, endIndex, 11)) return false;

                // 5. Break trendline: C[15] < leadInLevel
                double leadInLevel = C(bars, endIndex, 0) + trendSlope * 15.0;
                if (C(bars, endIndex, 15) >= leadInLevel) return false;

                return true;
            }

            public override List<GhostCandle> GenerateGhosts(
                Bars bars, int endIndex, double currentPrice)
            {
                double localATR = LocalATR(endIndex);
                double trendSlope = (C(bars, endIndex, 5) - C(bars, endIndex, 0)) / 5.0;
                // Trendline al centro del bump (pos. 8), non estrapolata fino a pos. 15:
                // cosi height = maxHigh - trendlineAtBump e sempre >= 0 per bump validi.
                double trendlineAtBump = C(bars, endIndex, 0) + trendSlope * 8.0;
                double maxHigh = double.MinValue;
                for (int i = 6; i <= 11; i++)
                    maxHigh = Math.Max(maxHigh, H(bars, endIndex, i));
                double height = maxHigh - trendlineAtBump;
                double cappedHeight = Math.Min(height, 2.5 * localATR);
                double target = currentPrice - cappedHeight;
                return GenerateGhostsToTarget(bars, endIndex, currentPrice, target, GhostCandleCount);
            }
        }

        // ============================================================
        // 25. ONSTART (parziale — struttura e indicatori)
        // ============================================================

        protected override void OnStart()
        {
            Print("[VIT] OnStart START");
            instanceId = "VPred_" + Guid.NewGuid().ToString("N").Substring(0, 8);

            // Initialize Pools
            _candidatePool = new ObjectPool<Candidate>(500, c => c.Reset());
            _ghostPool = new ObjectPool<GhostCandle>(5000, g => g.Reset());

            // Indicatori nativi
            _stoch = Indicators.StochasticOscillator(14, 3, 3, MovingAverageType.Simple);
            _elderRay = Indicators.ElderRayIndex(13,
                MovingAverageType.Exponential);

            // Squeeze Momentum
            _bbIndicator = Indicators.BollingerBands(
                Bars.ClosePrices, SQUEEZE_BB_PERIOD, SQUEEZE_BB_MULT,
                MovingAverageType.Simple);
            _kcMA = Indicators.MovingAverage(
                Bars.ClosePrices, SQUEEZE_KC_PERIOD,
                MovingAverageType.Exponential);
            _atrForKC = Indicators.AverageTrueRange(
                SQUEEZE_KC_PERIOD, MovingAverageType.Simple);

            _eom = Indicators.EaseOfMovement(14, MovingAverageType.Simple);
            _rsq = Indicators.LinearRegressionRSquared(Bars.ClosePrices, 14);
            _atr = Indicators.AverageTrueRange(ATR_PERIOD, MovingAverageType.Simple);
            _volumeSMA = Indicators.MovingAverage(Bars.TickVolumes, VOLUME_SMA_PERIOD, MovingAverageType.Simple);

            // DataSeries custom per Squeeze Momentum
            _squeezeMomSeries = CreateDataSeries();

            // Stato
            State = new PredictorState();
            ActiveCandidates = State.ActiveCandidates;
            GlobalAftermath = State.GlobalAftermath;
            IndicatorsCtx = State.Indicators;

            // Validazione parametri utente
            // AnalysisWindow ha gia MinValue = 2 nell'attributo Parameter,
            // cTrader impone il vincolo prima di Initialize: il guard e ridondante.
            if (CloneDistance < 1) CloneDistance = 1;

            // Clamp: DefaultGhostCount non puo superare AftermathCandles
            if (DefaultGhostCount > AftermathCandles)
                DefaultGhostCount = AftermathCandles;

            // Forza caricamento storico sufficiente (PRIMA del bias orario)
            int requiredBars = HistoryDepth + AnalysisWindow + AftermathCandles + 50;
            int loadIterations = 0;
            const int MAX_LOAD_ITERATIONS = 30;
            while (Bars.Count < requiredBars && loadIterations < MAX_LOAD_ITERATIONS)
            {
                loadIterations++;
                int before = Bars.Count;
                Bars.LoadMoreHistory();
                if (Bars.Count == before)
                {
                    Print($"Impossibile caricare piu storia. " +
                          $"Disponibili: {Bars.Count}, richieste: {requiredBars}");
                    break;
                }
            }
            if (loadIterations >= MAX_LOAD_ITERATIONS && Bars.Count < requiredBars)
            {
                Print($"Max iterazioni caricamento raggiunte ({MAX_LOAD_ITERATIONS}). " +
                      $"Disponibili: {Bars.Count}, richieste: {requiredBars}");
            }

            // Calcola periodo barra dinamicamente
            if (Bars.Count >= 2)
            {
                _barPeriodMinutes = (Bars.OpenTimes[Bars.Count - 1] -
                                     Bars.OpenTimes[Bars.Count - 2]).TotalMinutes;
                // Fallback se dati anomali (zero/negativo) o gap weekend (> 1 giorno)
                if (_barPeriodMinutes <= 0 || _barPeriodMinutes > 1440)
                    _barPeriodMinutes = GetTimeFrameMinutes();
                if (_barPeriodMinutes <= 0)
                    _barPeriodMinutes = 5; // fallback ultimo M5
            }
            else
                _barPeriodMinutes = 5; // fallback M5

            // Pre-popola _squeezeMomSeries per tutte le barre storiche
            // Evita che la LRF legga valori NaN/zero nelle prime ~20 barre live
            for (int idx = SQUEEZE_MOM_PERIOD; idx < Bars.Count; idx++)
            {
                double hh = double.MinValue;
                double ll = double.MaxValue;
                for (int j = idx - SQUEEZE_MOM_PERIOD + 1; j <= idx; j++)
                {
                    if (Bars.HighPrices[j] > hh) hh = Bars.HighPrices[j];
                    if (Bars.LowPrices[j] < ll) ll = Bars.LowPrices[j];
                }
                double mid = (hh + ll) / 2.0;
                _squeezeMomSeries[idx] = Bars.ClosePrices[idx] - mid;
            }

            // Pre-popola Elder History con le ultime 5 barre storiche
            // Evita che il contesto Elder sia tutto zero alle prime 5 candele live
            int lastHistBar = Bars.Count - 2; // ultima barra chiusa
            for (int i = 0; i < 5; i++)
            {
                int barIdx = lastHistBar - 4 + i;
                if (barIdx >= 0 && barIdx < Bars.Count)
                {
                    IndicatorsCtx.ElderBullHistory[i] = _elderRay.BullsPower[barIdx];
                    IndicatorsCtx.ElderBearHistory[i] = _elderRay.BearsPower[barIdx];
                }
            }

            // Pre-popola SqueezeIsOn_Prev per la prima detection live
            // Senza questo, SqueezeFired sarebbe sempre false alla prima detection
            if (Bars.Count >= 2)
            {
                int prevBar = Bars.Count - 2;
                double bbUp = _bbIndicator.Top[prevBar];
                double bbLo = _bbIndicator.Bottom[prevBar];
                double kcUp = _kcMA.Result[prevBar] + (_atrForKC.Result[prevBar] * SQUEEZE_KC_MULT);
                double kcLo = _kcMA.Result[prevBar] - (_atrForKC.Result[prevBar] * SQUEEZE_KC_MULT);
                IndicatorsCtx.SqueezeIsOn_Prev = bbUp < kcUp && bbLo > kcLo;
            }

            // Cache colori ghost (i parametri stringa non cambiano a runtime)
            _ghostBullColor = ParseColor(GhostBullColorStr);
            _ghostBearColor = ParseColor(GhostBearColorStr);
            _ghostNeutralColor = ParseColor(GhostNeutralColorStr);

            // HTF Initialization
            if (UseHtfFilter)
            {
                TimeFrame htfTf = ParseTimeFrame(HtfTimeframeStr);
                _htfBars = MarketData.GetBars(htfTf);
            }

            // Bias orario (DOPO il caricamento storia completa)
            BuildHourlyBias();

            // Pattern Library — registrazione pattern
            PatternDefinitions.Add(new BullFlagPattern(this));
            PatternDefinitions.Add(new BearFlagPattern(this));
            PatternDefinitions.Add(new DoubleBottomPattern(this));
            PatternDefinitions.Add(new DoubleTopPattern(this));
            PatternDefinitions.Add(new FallingWedgePattern(this));
            PatternDefinitions.Add(new RisingWedgePattern(this));
            PatternDefinitions.Add(new AscTrianglePattern(this));
            PatternDefinitions.Add(new DescTrianglePattern(this));
            PatternDefinitions.Add(new RisingThreeMethodsPattern(this));
            PatternDefinitions.Add(new FallingThreeMethodsPattern(this));
            PatternDefinitions.Add(new BullThreeLineStrikePattern(this));
            PatternDefinitions.Add(new BearThreeLineStrikePattern(this));
            PatternDefinitions.Add(new BullHikkakePattern(this));
            PatternDefinitions.Add(new BearHikkakePattern(this));
            PatternDefinitions.Add(new BullChannelBreakoutPattern(this));
            PatternDefinitions.Add(new BearChannelBreakoutPattern(this));
            PatternDefinitions.Add(new BullQuasimodoPattern(this));
            PatternDefinitions.Add(new BearQuasimodoPattern(this));
            PatternDefinitions.Add(new BullSpikeChannelPattern(this));
            PatternDefinitions.Add(new BearSpikeChannelPattern(this));
            PatternDefinitions.Add(new BullABCDPattern(this));
            PatternDefinitions.Add(new BearABCDPattern(this));
            PatternDefinitions.Add(new BullAdamEvePattern(this));
            PatternDefinitions.Add(new BearAdamEvePattern(this));
            PatternDefinitions.Add(new BullLiqGrabPattern(this));
            PatternDefinitions.Add(new BearLiqGrabPattern(this));
            PatternDefinitions.Add(new InverseHSPattern(this));
            PatternDefinitions.Add(new MiniHSPattern(this));
            PatternDefinitions.Add(new BullPennantPattern(this));
            PatternDefinitions.Add(new BearPennantPattern(this));
            PatternDefinitions.Add(new BullBroadeningWedgePattern(this));
            PatternDefinitions.Add(new BearBroadeningWedgePattern(this));
            PatternDefinitions.Add(new BullWolfeWavePattern(this));
            PatternDefinitions.Add(new BearWolfeWavePattern(this));
            PatternDefinitions.Add(new DiamondBottomPattern(this));
            PatternDefinitions.Add(new DiamondTopPattern(this));
            PatternDefinitions.Add(new BullThreeDrivesPattern(this));
            PatternDefinitions.Add(new BearThreeDrivesPattern(this));
            PatternDefinitions.Add(new PipeBottomPattern(this));
            PatternDefinitions.Add(new PipeTopPattern(this));
            PatternDefinitions.Add(new BullGartleyPattern(this));
            PatternDefinitions.Add(new BearGartleyPattern(this));
            PatternDefinitions.Add(new BullBatPattern(this));
            PatternDefinitions.Add(new BearBatPattern(this));
            PatternDefinitions.Add(new BullCrabPattern(this));
            PatternDefinitions.Add(new BearCrabPattern(this));
            PatternDefinitions.Add(new BullButterflyPattern(this));
            PatternDefinitions.Add(new BearButterflyPattern(this));
            PatternDefinitions.Add(new BullVolumeClimaxPattern(this));
            PatternDefinitions.Add(new BearVolumeClimaxPattern(this));
            PatternDefinitions.Add(new BullThreePushesPattern(this));
            PatternDefinitions.Add(new BearThreePushesPattern(this));
            PatternDefinitions.Add(new BullTrapSnapPattern(this));
            PatternDefinitions.Add(new BearTrapSnapPattern(this));
            PatternDefinitions.Add(new BullDoubleFakePattern(this));
            PatternDefinitions.Add(new BearDoubleFakePattern(this));
            PatternDefinitions.Add(new BullSharkPattern(this));
            PatternDefinitions.Add(new BearSharkPattern(this));
            PatternDefinitions.Add(new BullCypherPattern(this));
            PatternDefinitions.Add(new BearCypherPattern(this));
            PatternDefinitions.Add(new Bull50Pattern(this));
            PatternDefinitions.Add(new Bear50Pattern(this));
            PatternDefinitions.Add(new BullAltBatPattern(this));
            PatternDefinitions.Add(new BearAltBatPattern(this));
            PatternDefinitions.Add(new BullRossHookPattern(this));
            PatternDefinitions.Add(new BearRossHookPattern(this));
            PatternDefinitions.Add(new Bull2BPattern(this));
            PatternDefinitions.Add(new Bear2BPattern(this));
            PatternDefinitions.Add(new BullTurtleSoupPattern(this));
            PatternDefinitions.Add(new BearTurtleSoupPattern(this));
            PatternDefinitions.Add(new BullBreakerPattern(this));
            PatternDefinitions.Add(new BearBreakerPattern(this));
            PatternDefinitions.Add(new BullFVGPattern(this));
            PatternDefinitions.Add(new BearFVGPattern(this));
            PatternDefinitions.Add(new BullTwoLegPBPattern(this));
            PatternDefinitions.Add(new BearTwoLegPBPattern(this));
            PatternDefinitions.Add(new BullBumpRunPattern(this));
            PatternDefinitions.Add(new BearBumpRunPattern(this));

            // Timer
            if (RefreshSeconds < 1) RefreshSeconds = 15;
            Timer.Start(TimeSpan.FromSeconds(RefreshSeconds));
            Print($"[VIT] OnStart DONE. Bars: {Bars.Count}, BarPeriod: {_barPeriodMinutes}min");
        }

        // ============================================================
        // 26. ONTICK / ONBAR — ENTRY POINTS (Robot)
        // ============================================================

        protected override void OnTick()
        {
            try
            {
                int index = Bars.Count - 1;
                int minRequired = AnalysisWindow + AftermathCandles + VOLUME_SMA_PERIOD + 50;
                if (index < minRequired)
                    return;

                // === Retry apertura pendente (se il ciclo precedente è fallito) ===
                if (_pendingOpenSignal != null)
                {
                    // Solo se il segnale pending è ancora quello attivo e valido
                    if (_pendingOpenSignal == State.ActiveSignal
                        && State.ActiveSignal != null
                        && State.ActiveSignal.IsValid)
                    {
                        // Limite di sicurezza anti-loop estremo
                        if (_pendingOpenAttempts < 500)
                        {
                            TryExecuteTrade();
                        }
                        else
                        {
                            Print($"[VIT BOT] Rinuncia retry dopo {_pendingOpenAttempts} tentativi: segnale ignorato");
                            _pendingOpenSignal = null;
                            _pendingOpenAttempts = 0;
                        }
                    }
                    else
                    {
                        // Il segnale pending è cambiato/invalidato: annulla retry
                        _pendingOpenSignal = null;
                        _pendingOpenAttempts = 0;
                    }
                }

                // === Primo tick "live": scan iniziale completo ===
                // (equivalente al ramo "LIVE DETECTED" del vecchio Calculate)
                if (!_firstLiveBarDone)
                {
                    _firstLiveBarDone = true;
                    Print($"[VIT] LIVE DETECTED: index={index}, Bars.Count={Bars.Count}");

                    UpdateNativeIndicators(index);

                    // Salva i Prev per il ciclo Squeeze (come in Calculate originale)
                    IndicatorsCtx.SqueezeMomentum_Prev = IndicatorsCtx.SqueezeMomentum;
                    IndicatorsCtx.SqueezeIsOn_Prev = IndicatorsCtx.SqueezeIsOn;

                    ExecuteFullScan(index);
                    Print($"[VIT] Scan completato. Candidati: {ActiveCandidates.Count}, " +
                          $"Posizioni scansionate: {State.TotalPositionsScanned}");
                    UpdateDirectionalContext();
                    UpdateFavorite();
                    TryCreateSignal();
                    Print($"[VIT] Favorito: {(State.CurrentFavorite != null ? State.CurrentFavorite.PatternName : "nessuno")}, " +
                          $"Segnale: {(State.ActiveSignal != null && State.ActiveSignal.IsValid ? "SI" : "NO")}");
                    RenderGhosts();
                    RenderPanel();
                    Print("[VIT] Rendering completato.");
                    _lastIndex = index;
                    return;
                }

                // === Barra live: aggiorna indicatori ad ogni tick (come in Calculate) ===
                UpdateNativeIndicators(index);
                _lastIndex = index;
            }
            catch (Exception ex)
            {
                Print($"[VIT] ERRORE in OnTick: {ex.Message}\n{ex.StackTrace}");
            }
        }

        protected override void OnBar()
        {
            try
            {
                int index = Bars.Count - 1;
                int minRequired = AnalysisWindow + AftermathCandles + VOLUME_SMA_PERIOD + 50;
                if (index < minRequired)
                    return;

                // Se non abbiamo ancora fatto lo scan iniziale, sarà OnTick a farlo
                if (!_firstLiveBarDone)
                    return;

                // Aggiornare gli indicatori nativi PRIMA di OnBarClosed preserva l'ordine
                // originale (in Calculate, UpdateNativeIndicators veniva chiamato prima).
                // OnBarClosedInternal dentro usa valori indicatore live in UpdateDirectionalContext.
                UpdateNativeIndicators(index);
                OnBarClosed(index);

                // Safety net: il PASSO 2 di OnBarClosedInternal può aver invalidato il
                // segnale (GhostsRemaining → 0) senza che venga creato un sostituto.
                // Sincronizza la posizione col nuovo stato del segnale.
                SyncPositionWithSignal();

                _lastIndex = index;
            }
            catch (Exception ex)
            {
                Print($"[VIT] ERRORE in OnBar: {ex.Message}\n{ex.StackTrace}");
            }
        }

        // ============================================================
        // 17. CHIUSURA CANDELA — ORDINE OPERAZIONI
        // ============================================================

        private void OnBarClosed(int index)
        {
            if (_isProcessing) return;
            _isProcessing = true;
            try
            {
            OnBarClosedInternal(index);
            }
            finally
            {
                _isProcessing = false;
            }
        }

        private void OnBarClosedInternal(int index)
        {
            // PASSO 1 — Conferma meccanica sulla candela appena chiusa
            // realFP e costante per tutte le iterazioni (argomento index - 1):
            // calcolarlo una volta sola fuori dal loop.
            var realFP = BuildFingerprint(index - 1);
            foreach (var c in ActiveCandidates.Where(c => c.IsActive).ToList())
            {
                int ghostIndex = c.CandlesElapsed;
                if (ghostIndex < c.GhostCandles.Count)
                {
                    var ghostRef = c.GhostCandles[ghostIndex];
                    var ghostFP = BuildFingerprintFromGhost(ghostRef);
                    double matchScore = CalculateCandleSimilarity(realFP, ghostFP);

                    double delta = (matchScore - 0.5) * RT_DELTA_MULTIPLIER_CLOSE;

                    // Volume Multiplier: stessa logica di OnTimer.
                    // Il volume della candela reale appena chiusa amplifica/smorza
                    // il delta. Cap tra 0.3 e 2.0.
                    double volMultiplier = Math.Max(0.3,
                        Math.Min(realFP.VolumeRelative, 2.0));
                    delta *= volMultiplier;

                    c.RealtimeScore = Math.Max(0, Math.Min(100,
                        c.RealtimeScore + delta));

                    RecalculateConfidence(c);
                }
                c.CandlesElapsed++;
            }

            // PASSO 1.5 — Disattiva candidati con ghost esaurite
            foreach (var c in ActiveCandidates.Where(c => c.IsActive).ToList())
            {
                if (c.CandlesElapsed >= c.GhostCandles.Count)
                    c.IsActive = false;
            }

            // PASSO 2 — Decrementa ghost rimanenti del segnale attivo
            if (State.ActiveSignal != null && State.ActiveSignal.IsValid)
            {
                State.ActiveSignal.GhostsRemaining--;
                if (State.ActiveSignal.GhostsRemaining <= 0)
                    State.ActiveSignal.IsValid = false;
            }

            // PASSO 3 — Eliminazione sotto soglia
            EliminateBelowThreshold();

            // PASSO 4.5 — Shift storia indicatori (SOLO qui, mai a ogni tick)
            // index e la nuova barra, index-1 e la barra appena chiusa
            ShiftIndicatorHistory(index - 1);

            // PASSO 5 — Scan completo
            ExecuteFullScan(index);

            // PASSO 6 — Decay candidati non ritrovati
            foreach (var c in ActiveCandidates.Where(c => c.IsActive).ToList())
            {
                if (!c.WasFoundInLastScan)
                {
                    c.InitialSimilarity *= DecayFactor;
                    RecalculateConfidence(c);
                }
            }

            // PASSO 7 — Eliminazione post-decay
            EliminateBelowThreshold();

            // PASSO 8 — Reset totale se tutti eliminati
            // Dopo PASSO 7 (EliminateBelowThreshold) la lista contiene SOLO
            // candidati attivi: basta controllare Count == 0.
            if (ActiveCandidates.Count == 0)
            {
                HandleReset();
            }

            // PASSO 9 — Aggiorna contesto direzionale (aftermath fresco dallo scan)
            UpdateDirectionalContext();

            // PASSO 10 — Aggiorna favorito e prova a creare segnale
            UpdateFavorite();
            TryCreateSignal();

            // PASSO 11 — Rendering
            RenderGhosts();
            RenderPanel();
        }

        // ============================================================
        // 8. BIAS ORARIO
        // ============================================================

        private void BuildHourlyBias()
        {
            // Per ogni fascia oraria h (0-23 UTC, corretta con BiasUTCOffset):
            // Conta nelle ultime BiasLookbackDays x 288 candele M5
            int barsPerDay = (_barPeriodMinutes > 0)
                ? (int)(1440.0 / _barPeriodMinutes) : 288;
            int lookbackCandles = BiasLookbackDays * barsPerDay;
            int startBar = Math.Max(0, Bars.Count - 1 - lookbackCandles);

            // Array per conteggio per ogni slot orario
            int[] longCount = new int[24];
            int[] shortCount = new int[24];
            int[] totalSignificant = new int[24];

            for (int bar = startBar; bar < Bars.Count - 1; bar++)
            {
                int hourSlot = GetHourSlot(Bars.OpenTimes[bar]);

                // Movimento della candela
                double move = Bars.ClosePrices[bar] - Bars.OpenPrices[bar];
                double absMove = Math.Abs(move);

                // ATR storico per questa posizione
                double atr = CalcolaATR(bar);
                if (atr <= 0) continue;

                double threshold = MovementATRMultiplier * atr;

                // Solo candele con movimento significativo
                if (absMove >= threshold)
                {
                    totalSignificant[hourSlot]++;
                    if (move > 0)
                        longCount[hourSlot]++;
                    else
                        shortCount[hourSlot]++;
                }
            }

            // Determina bias per ogni slot
            for (int h = 0; h < 24; h++)
            {
                if (totalSignificant[h] == 0)
                {
                    _hourlyBias[h] = "NEUTRAL";
                    continue;
                }

                double longPct = (double)longCount[h] / totalSignificant[h];
                double shortPct = (double)shortCount[h] / totalSignificant[h];

                if (longPct > 0.60)
                    _hourlyBias[h] = "BUY";
                else if (shortPct > 0.60)
                    _hourlyBias[h] = "SELL";
                else
                    _hourlyBias[h] = "NEUTRAL";
            }
        }

        // ============================================================
        // 16. RIVALUTAZIONE 15 SECONDI (OnTimer)
        // ============================================================

        protected override void OnTimer()
        {
            if (_isProcessing) return;
            if (!_firstLiveBarDone) return;
            _isProcessing = true;
            try
            {
            // Rivalidazione segnale ANCHE senza candidati attivi
            // (evita segnali "zombie" dopo HandleReset)
            RevalidateSignal();

            // === BOT: safety net immediato dopo RevalidateSignal ===
            // RevalidateSignal può aver invalidato il segnale (SL hit, deviazione,
            // cambio direzione) e il TryCreateSignal chiamato internamente potrebbe
            // non aver creato un sostituto (favorito null, confidence bassa, ecc.).
            // Chiudi subito la posizione orfana senza aspettare il prossimo ciclo.
            SyncPositionWithSignal();

            if (ActiveCandidates.Count == 0)
            {
                RenderGhosts();
                RenderPanel();
                return;
            }

            int currentBar = Bars.Count - 1; // candela in formazione

            // currentBar e lo stesso per tutti i candidati: calcola il
            // fingerprint UNA volta fuori dal loop, non N volte dentro.
            var currentFP = BuildFingerprint(currentBar);

            // Normalizza VolumeRelative per la frazione di barra trascorsa:
            // il volume si accumula durante la formazione della candela,
            // quindi all'inizio della barra e sistematicamente basso e il
            // volMultiplier smorzerebbe erroneamente il delta. Dividendo
            // per la frazione temporale, il volume viene proiettato al
            // valore atteso a fine barra. Evita un bias temporale a favore
            // dei tick tardivi nella candela.
            double elapsedMinutes = (Server.TimeInUtc - Bars.OpenTimes[currentBar]).TotalMinutes;
            double elapsedFraction = (_barPeriodMinutes > 0)
                ? elapsedMinutes / _barPeriodMinutes : 1.0;
            if (elapsedFraction > 0.01 && elapsedFraction <= 1.0)
                currentFP.VolumeRelative /= elapsedFraction;

            // Un solo loop per (a) aggiornare RT scores dei candidati con
            // ghost ancora disponibili e (b) marcare come inattivi quelli
            // che hanno esaurito tutte le ghost. Evita la seconda
            // allocazione ToList() con lo stesso filtro.
            foreach (var c in ActiveCandidates.Where(c => c.IsActive).ToList())
            {
                int ghostIndex = c.CandlesElapsed;
                if (ghostIndex >= c.GhostCandles.Count)
                {
                    c.IsActive = false; // ghost esaurite → disattiva
                    continue;
                }

                var ghostRef = c.GhostCandles[ghostIndex];
                var ghostFP = BuildFingerprintFromGhost(ghostRef);
                double matchScore = CalculateCandleSimilarity(currentFP, ghostFP);

                double delta = (matchScore - 0.5) * RT_DELTA_MULTIPLIER_TICK;

                // Volume Multiplier: il volume della candela reale amplifica
                // o smorza il delta. Volume doppio della media -> delta raddoppiato.
                // Volume basso -> delta ridotto. Cap tra 0.3 e 2.0.
                double volMultiplier = Math.Max(0.3,
                    Math.Min(currentFP.VolumeRelative, 2.0));
                delta *= volMultiplier;

                c.RealtimeScore = Math.Max(0, Math.Min(100,
                    c.RealtimeScore + delta));

                RecalculateConfidence(c);
            }

            // Eliminazione sotto soglia
            EliminateBelowThreshold();

            // Reset totale se tutti eliminati. Dopo EliminateBelowThreshold
            // la lista contiene SOLO candidati attivi: basta Count == 0.
            if (ActiveCandidates.Count == 0)
            {
                HandleReset();
                // Safety net: HandleReset ha invalidato il segnale. Chiudi la posizione
                // prima di uscire, altrimenti resterebbe orfana fino al prossimo ciclo.
                SyncPositionWithSignal();
                RenderGhosts();
                RenderPanel();
                return;
            }

            // Aggiorna contesto direzionale
            UpdateDirectionalContext();

            // Aggiorna favorito (usa EffectiveConfidence)
            UpdateFavorite();

            // Creazione segnale mid-bar se non esiste un segnale attivo.
            // La RevalidateSignal() all'inizio di OnTimer ha gia invalidato
            // eventuali segnali disallineati (direzione / deviazione / SL)
            // chiamando al suo interno TryCreateSignal() per la "Coda
            // segnali istantanea". Qui serve solo come fallback nel caso
            // raro in cui RevalidateSignal non abbia potuto agire (es.
            // CurrentFavorite era null all'inizio ed e stato riempito ora
            // da UpdateFavorite dopo UpdateDirectionalContext).
            TryCreateSignal();

            // === BOT: safety net per chiusura su scomparsa del segnale ===
            // Se HandleReset o altri punti hanno invalidato il segnale senza creare un
            // sostituto, la posizione resterebbe aperta. Questo check periodico (ogni
            // RefreshSeconds, default 15s) la chiude.
            SyncPositionWithSignal();

            // NOTA (fix): NON chiamare una seconda RevalidateSignal() qui.
            // Il segnale appena creato da TryCreateSignal userebbe il
            // favorito corrente: rivalidarlo immediatamente rischia di
            // invalidarlo per deviazione vs ghost[0] (calcolata sulla
            // chiusa precedente) o per un micro-cambio di direzione,
            // uccidendo un segnale con zero tempo di vita e resettando
            // GhostsRemaining al prossimo ciclo. Lasciamo che la
            // RevalidateSignal() del prossimo OnTimer (o di OnBar) faccia
            // il check con dati coerenti.

            // Rendering
            RenderGhosts();
            RenderPanel();
            }
            finally
            {
                _isProcessing = false;
            }
        }

        // ============================================================
        // 21. RIVALIDAZIONE SEGNALE ATTIVO
        // ============================================================

        private void RevalidateSignal()
        {
            if (State.ActiveSignal == null || !State.ActiveSignal.IsValid) return;

            var signal = State.ActiveSignal;
            double currentPrice = Bars.ClosePrices.LastValue;

            // 0. Favorito cambia direzione → segnale invalidato
            if (State.CurrentFavorite != null)
            {
                bool signalIsLong = signal.Outcome == AftermathOutcome.Long;
                bool signalIsShort = signal.Outcome == AftermathOutcome.Short;
                bool signalIsRange = signal.Outcome == AftermathOutcome.Range;

                bool favIsLong = State.CurrentFavorite.PredictedOutcome == AftermathOutcome.Long;
                bool favIsShort = State.CurrentFavorite.PredictedOutcome == AftermathOutcome.Short;
                bool favIsRange = State.CurrentFavorite.PredictedOutcome == AftermathOutcome.Range;

                bool directionChanged = false;

                if (signalIsLong && !favIsLong) directionChanged = true;
                if (signalIsShort && !favIsShort) directionChanged = true;
                if (signalIsRange && !favIsRange) directionChanged = true;

                if (directionChanged)
                {
                    // Time-Lock: impedisce il cambio se il segnale è troppo giovane
                    var lifeTime = (Server.TimeInUtc - State.SignalCreationTime).TotalSeconds;
                    if (lifeTime >= MinSignalLifeSeconds)
                    {
                        signal.IsValid = false;
                        TryCreateSignal();
                        return;
                    }
                }
            }

            // 1. Deviazione dal percorso ghost
            int ghostIdx = signal.SignalGhosts.Count - signal.GhostsRemaining;
            // Clamp per sicurezza quando GhostsRemaining è 0
            if (ghostIdx >= signal.SignalGhosts.Count)
                ghostIdx = signal.SignalGhosts.Count - 1;
            if (ghostIdx >= 0 && ghostIdx < signal.SignalGhosts.Count)
            {
                var expectedGhost = signal.SignalGhosts[ghostIdx];
                double expectedMid = (expectedGhost.High + expectedGhost.Low) / 2;
                double ghostRange = expectedGhost.High - expectedGhost.Low;
                if (ghostRange < 0.0001) ghostRange = 0.0001;
                double deviation = Math.Abs(currentPrice - expectedMid) / ghostRange;

                if (deviation > DeviationThreshold)
                {
                    // Time-Lock secondario: non invalidare per deviazione nei primi N secondi
                    var lifeTime = (Server.TimeInUtc - State.SignalCreationTime).TotalSeconds;
                    if (lifeTime >= MinSignalLifeSeconds)
                    {
                        signal.IsValid = false;
                    }
                    // NON fare return qui — il flusso deve raggiungere
                    // la coda segnali istantanea in fondo alla funzione
                }
            }

            // 2. Stop loss toccato (solo per LONG/SHORT)
            if (signal.Outcome != AftermathOutcome.Range && signal.StopLossLevel > 0)
            {
                if (signal.IsBuy && currentPrice <= signal.StopLossLevel)
                    signal.IsValid = false;
                else if (!signal.IsBuy && currentPrice >= signal.StopLossLevel)
                    signal.IsValid = false;
            }

            // NOTA: il controllo "ghost esaurite" (GhostsRemaining <= 0)
            // e gestito nella Sezione 17, Passo 2, a chiusura candela.
            // Conta candele chiuse reali, NON minuti passati.
            // Cosi il segnale non muore durante weekend o pause di mercato.

            // 3. Coda Segnali Istantanea: se il segnale e stato invalidato
            //    (per deviazione o SL toccato), prova subito a crearne uno
            //    nuovo dal favorito corrente. Il trader non resta senza
            //    segnale per quasi 5 minuti. TryCreateSignal() ha gia il
            //    guard "if (ActiveSignal != null && ActiveSignal.IsValid) return;"
            //    quindi non crea duplicati se il segnale e ancora valido.
            if (!signal.IsValid)
            {
                TryCreateSignal();
            }
        }

        // ============================================================
        // 22. RENDERING — GHOST CANDLES
        // ============================================================

        private void RenderGhosts()
        {
            // Rimuovi solo gli oggetti ghost che abbiamo effettivamente
            // disegnato noi (tracciati in _renderedGhostNames), evitando
            // di iterare su tutti gli oggetti del chart ad ogni render.
            foreach (var name in _renderedGhostNames)
                Chart.RemoveObject(name);
            _renderedGhostNames.Clear();

            Chart.RemoveObject(instanceId + "_sl");
            Chart.RemoveObject(instanceId + "_tp");
            Chart.RemoveObject(instanceId + "_wc");
            Chart.RemoveObject(instanceId + "_rh");
            Chart.RemoveObject(instanceId + "_rl");

            List<GhostCandle> ghostsToDraw;
            Color bullColor, bearColor;
            bool isSignal = false;

            if (State.ActiveSignal != null && State.ActiveSignal.IsValid)
            {
                ghostsToDraw = State.ActiveSignal.SignalGhosts;
                bullColor = _ghostBullColor;
                bearColor = _ghostBearColor;
                isSignal = true;
            }
            else if (State.CurrentFavorite != null)
            {
                ghostsToDraw = State.CurrentFavorite.GhostCandles;
                bullColor = _ghostNeutralColor;
                bearColor = _ghostNeutralColor;
            }
            else return;

            // Per il segnale, disegna solo le ghost rimanenti (non quelle passate)
            // Per il favorito, parti da CandlesElapsed
            int startIdx = 0;
            if (isSignal)
                startIdx = ghostsToDraw.Count - State.ActiveSignal.GhostsRemaining;
            else if (State.CurrentFavorite != null)
                startIdx = State.CurrentFavorite.CandlesElapsed;
            if (startIdx < 0) startIdx = 0;

            for (int i = startIdx; i < ghostsToDraw.Count; i++)
            {
                var ghost = ghostsToDraw[i];
                DateTime ghostTime = ghost.ProjectedTime;
                DateTime ghostEnd = ghostTime + TimeSpan.FromMinutes(_barPeriodMinutes);
                string prefix = instanceId + "_ghost_" + i;

                Color color = ghost.IsBullish ? bullColor : bearColor;

                double bodyTop = Math.Max(ghost.Open, ghost.Close);
                double bodyBottom = Math.Min(ghost.Open, ghost.Close);
                string bodyName = prefix + "_body";
                var rect = Chart.DrawRectangle(bodyName,
                    ghostTime, bodyTop, ghostEnd, bodyBottom, color);
                rect.IsFilled = true;
                _renderedGhostNames.Add(bodyName);

                DateTime wickX = ghostTime + TimeSpan.FromMinutes(_barPeriodMinutes / 2.0);
                string wickUpName = prefix + "_wu";
                Chart.DrawTrendLine(wickUpName,
                    wickX, bodyTop, wickX, ghost.High,
                    color, 1, LineStyle.Solid);
                _renderedGhostNames.Add(wickUpName);

                string wickLowName = prefix + "_wl";
                Chart.DrawTrendLine(wickLowName,
                    wickX, bodyBottom, wickX, ghost.Low,
                    color, 1, LineStyle.Solid);
                _renderedGhostNames.Add(wickLowName);
            }

            if (isSignal && State.ActiveSignal.Outcome != AftermathOutcome.Range)
            {
                if (State.ActiveSignal.StopLossLevel > 0)
                {
                    Chart.DrawHorizontalLine(instanceId + "_sl",
                        State.ActiveSignal.StopLossLevel,
                        Color.Red, 1, LineStyle.Dots);
                }
                if (State.ActiveSignal.TakeProfitLevel > 0)
                {
                    Chart.DrawHorizontalLine(instanceId + "_tp",
                        State.ActiveSignal.TakeProfitLevel,
                        Color.Green, 1, LineStyle.Dots);
                }
                // Worst Case Drawdown: linea arancione tratteggiata
                // Mostra il 95° percentile del drawdown storico.
                // Per LONG: sotto lo SL. Per SHORT: sopra lo SL.
                // Informazione pura — non blocca nessun segnale.
                if (State.ActiveSignal.WorstCaseLevel > 0)
                {
                    Chart.DrawHorizontalLine(instanceId + "_wc",
                        State.ActiveSignal.WorstCaseLevel,
                        Color.Orange, 1, LineStyle.Dots);
                }
            }

            if (isSignal && State.ActiveSignal.Outcome == AftermathOutcome.Range)
            {
                if (State.ActiveSignal.RangeHigh > 0)
                {
                    Chart.DrawHorizontalLine(instanceId + "_rh",
                        State.ActiveSignal.RangeHigh,
                        Color.DodgerBlue, 1, LineStyle.Dots);
                }
                if (State.ActiveSignal.RangeLow > 0)
                {
                    Chart.DrawHorizontalLine(instanceId + "_rl",
                        State.ActiveSignal.RangeLow,
                        Color.DodgerBlue, 1, LineStyle.Dots);
                }
            }
        }

        private void ClearGhostRendering()
        {
            // Rimuovi gli oggetti ghost tracciati (evita l'iterazione su
            // Chart.Objects) e svuota il set. Le linee SL/TP/WC/RH/RL
            // sono rimosse per nome esatto — non serve iterare.
            foreach (var name in _renderedGhostNames)
                Chart.RemoveObject(name);
            _renderedGhostNames.Clear();

            Chart.RemoveObject(instanceId + "_sl");
            Chart.RemoveObject(instanceId + "_tp");
            Chart.RemoveObject(instanceId + "_wc");
            Chart.RemoveObject(instanceId + "_rh");
            Chart.RemoveObject(instanceId + "_rl");
        }

        // ============================================================
        // 23. RENDERING — PANNELLO
        // ============================================================

        private void RenderPanel()
        {
            if (!ShowPanel) return;

            int hourSlot = GetCurrentHourSlot();
            string panelText = "";

            // Riga 1
            panelText += "VIT Predittore v2.2\n";

            // Riga 2 — Fascia oraria
            string currentBias = GetBiasLabel(hourSlot);
            string htfStatus = UseHtfFilter ? (IsHtfTrendAligned(true) ? "HTF:BUY" : (IsHtfTrendAligned(false) ? "HTF:SELL" : "HTF:WAIT")) : "HTF:OFF";
            panelText += $"Fascia: {hourSlot}:00-{(hourSlot + 1) % 24}:00 [{currentBias}] | {htfStatus}\n";

            // Riga 3 — Posizioni scansionate
            panelText += $"Scansionate: {State.TotalPositionsScanned}\n";

            // Riga 3b — Storico caricato
            panelText += $"Storico: {Bars.Count} caricate / {HistoryDepth} target\n";

            // Riga 4 — Candidati attivi (storici + libreria)
            int nHist = ActiveCandidates.Count(c =>
                c.IsActive && c.Source == CandidateSource.Historical);
            int nLib = ActiveCandidates.Count(c =>
                c.IsActive && c.Source == CandidateSource.PatternLibrary);
            int activeCount = nHist + nLib;
            panelText += $"Candidati: {activeCount} " +
                         $"({nHist}H + {nLib}L)\n";

            // Riga 5 — Counter BUY/SELL/RANGE (escludi Range dai conteggi direzionali)
            int countBuy = ActiveCandidates.Count(c =>
                c.IsActive && c.PredictedOutcome != AftermathOutcome.Range && c.IsBuy);
            int countSell = ActiveCandidates.Count(c =>
                c.IsActive && c.PredictedOutcome != AftermathOutcome.Range && !c.IsBuy);
            int countRange = ActiveCandidates.Count(c =>
                c.IsActive && c.PredictedOutcome == AftermathOutcome.Range);
            panelText += $"Counter: {countBuy} BUY / {countSell} SELL / {countRange} RANGE\n";

            int totalDir = countBuy + countSell;
            int effCancel = Math.Max(CancellationDiff, (int)(totalDir * CancellationRatio));
            panelText += $"Cancel: {effCancel} (base {CancellationDiff}, dir {totalDir}×{CancellationRatio:F2})\n";

            // Riga 6 — Aftermath (tre esiti percentuali)
            panelText += $"Aftermath: {(int)State.PctLong}%L " +
                         $"{(int)State.PctShort}%S {(int)State.PctRange}%R " +
                         $"({State.GlobalAftermath.TotalOccurrences} occ)\n";

            // Riga 7 — Favorito
            if (State.CurrentFavorite != null)
            {
                var f = State.CurrentFavorite;
                string dir = f.IsBuy ? "BUY" : "SELL";
                if (f.PredictedOutcome == AftermathOutcome.Range) dir = "RANGE";
                panelText += $"Favorito: {f.PatternName} " +
                             $"({f.ConfidenceScore:F1}%) {dir}\n";
            }
            else
                panelText += "Favorito: nessuno\n";

            // Riga 8 — Contesto direzionale (contributi individuali)
            var ctx = State.Context;
            panelText += $"Ctx: St{ctx.StochContrib:+0.00;-0.00} " +
                         $"El{ctx.ElderContrib:+0.00;-0.00} " +
                         $"Sq{ctx.SqueezeContrib:+0.00;-0.00} " +
                         $"Em{ctx.EOMContrib:+0.00;-0.00}\n";

            // Riga 9 — Contesto: bias, aftermath, R², risultato finale
            panelText += $"Ctx: B{ctx.HourlyBiasContrib:+0.00;-0.00} " +
                         $"A{ctx.AftermathBiasContrib:+0.00;-0.00} " +
                         $"R²×{ctx.RSquaredMultiplier:F1} " +
                         $"→{ctx.FinalContext:+0.00;-0.00}\n";

            // Riga 10 — Indicatori raw (sintesi compatta)
            panelText += $"Stoch:{IndicatorsCtx.StochK:F0}/{IndicatorsCtx.StochD:F0} " +
                         $"R²:{IndicatorsCtx.RSquared:F2} " +
                         $"Sqz:{(IndicatorsCtx.SqueezeIsOn ? "ON" : "off")}\n";

            // Riga 11 — Segnale
            if (State.ActiveSignal != null && State.ActiveSignal.IsValid)
            {
                var s = State.ActiveSignal;
                if (s.Outcome == AftermathOutcome.Range)
                {
                    panelText += $"SEGNALE: RANGE " +
                                 $"[{s.RangeLow:F2}-{s.RangeHigh:F2}] " +
                                 $"| Ghost: {s.GhostsRemaining}/" +
                                 $"{s.SignalGhosts.Count}\n";
                }
                else
                {
                    string sDir = s.IsBuy ? "BUY" : "SELL";
                    panelText += $"SEGNALE: {sDir} | SL:{s.StopLossLevel:F2} " +
                                 $"WC:{s.WorstCaseLevel:F2} " +
                                 $"TP:{s.TakeProfitLevel:F2} " +
                                 $"R:R {s.RiskReward:F1} " +
                                 $"| Ghost: {s.GhostsRemaining}/" +
                                 $"{s.SignalGhosts.Count}\n";
                }
            }
            else
                panelText += "Nessun segnale attivo\n";

            // Riga 12 — Ultimo scan
            DateTime displayTime = State.LastFullScanTime
                .AddHours(PanelUTCOffset);
            panelText += $"Scan: {displayTime:HH:mm:ss}\n";

            // Reset state
            if (State.IsInResetState)
                panelText += "RESET — attesa prossimo scan\n";

            // Warning timeframe
            if (Bars.TimeFrame != TimeFrame.Minute5)
                panelText += "⚠ USA M5!\n";

            Chart.DrawStaticText(instanceId + "_panel", panelText,
                GetVerticalAlignment(), GetHorizontalAlignment(),
                Color.White);
        }
    }
}
