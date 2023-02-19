/*
Copyright 2023 Boonebytes

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

namespace DiscordBot.Infrastructure.Repositories;

public partial class AllianceRepository
{
    internal const string SqlSelectTerritoryInventory = @"
SELECT
        EFFECTIVE_DATE          EffectiveDate,
        REFINED_ISO1            Isogen1,
        DELTA_ISO1              Isogen1Delta,
        REFINED_ISO2            Isogen2,
        DELTA_ISO2              Isogen2Delta,
        REFINED_ISO3            Isogen3,
        DELTA_ISO3              Isogen3Delta,
        PROGENITOR_EMITTERS     Emitters,
        DELTA_EMITTERS          EmittersDelta,
        PROGENITOR_DIODES       Diodes,
        DELTA_DIODES            DiodesDelta,
        PROGENITOR_CORES        Cores,
        DELTA_CORES             CoresDelta,
        PROGENITOR_REACTORS     Reactors,
        DELTA_REACTORS          ReactorsDelta
        -- COLLISIONAL_PLASMA
        -- DELTA_COLLISIONAL
        -- MAGNETIC_PLASMA
        -- DELTA_MAGNETIC
        -- SUBSPACE_SUPERCONDUCTOR
        -- DELTA_SUPERC
        -- ALLIANCE_RESERVES
        -- DELTA_RESERVES
    FROM V_ALLIANCE_INVENTORY_DELTAS
    WHERE alliance_id = :AllianceId
    AND effective_date > sysdate - (7*12)
    ORDER BY effective_date DESC
";
    
    internal const string SqlSelectStarbaseInventory = @"
SELECT
        EFFECTIVE_DATE          EffectiveDate,
        COLLISIONAL_PLASMA      CollisionalPlasma,
        DELTA_COLLISIONAL       CollisionalPlasmaDelta,
        MAGNETIC_PLASMA         MagneticPlasma,
        DELTA_MAGNETIC          MagneticPlasmaDelta,
        SUBSPACE_SUPERCONDUCTOR Superconductors,
        DELTA_SUPERC            SuperconductorsDelta,
        ALLIANCE_RESERVES       Reserves,
        DELTA_RESERVES          ReservesDelta
    FROM V_ALLIANCE_INVENTORY_DELTAS
    WHERE alliance_id = :AllianceId
    AND effective_date > sysdate - (7*12)
    ORDER BY effective_date DESC
";
    
}