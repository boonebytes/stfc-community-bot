/*
Copyright 2022 Boonebytes

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

create or replace view STFC.V_ALLIANCE_INVENTORY_DELTAS as
SELECT
    latest.alliance_id,
    latest.effective_date, --previous.effective_date
    latest.refined_iso1,
    latest.refined_iso1 - previous.refined_iso1 delta_iso1,
    latest.refined_iso2,
    latest.refined_iso2 - previous.refined_iso2 delta_iso2,
    latest.refined_iso3,
    latest.refined_iso3 - previous.refined_iso3 delta_iso3,
    latest.Progenitor_Emitters,
    latest.Progenitor_Emitters - previous.Progenitor_Emitters delta_emitters,
    latest.Progenitor_Diodes,
    latest.Progenitor_Diodes - previous.Progenitor_Diodes delta_diodes,
    latest.Progenitor_Cores,
    latest.Progenitor_Cores - previous.Progenitor_Cores delta_cores,
    latest.Progenitor_Reactors,
    latest.Progenitor_Reactors - previous.Progenitor_Reactors delta_reactors,
    latest.COLLISIONAL_PLASMA,
    latest.COLLISIONAL_PLASMA - previous.COLLISIONAL_PLASMA delta_collisional,
    latest.MAGNETIC_PLASMA,
    latest.MAGNETIC_PLASMA - previous.MAGNETIC_PLASMA delta_magnetic,
    latest.SUBSPACE_SUPERCONDUCTOR,
    latest.SUBSPACE_SUPERCONDUCTOR - previous.SUBSPACE_SUPERCONDUCTOR delta_superc,
    latest.ALLIANCE_RESERVES,
    latest.ALLIANCE_RESERVES - previous.ALLIANCE_RESERVES delta_reserves
FROM V_ALLIANCE_INVENTORY latest
LEFT OUTER JOIN V_ALLIANCE_INVENTORY previous
ON (
    previous.alliance_id = latest.alliance_id
    AND previous.effective_date < latest.effective_date
    AND NOT EXISTS (
        SELECT 1
        FROM V_ALLIANCE_INVENTORY cpr
        WHERE cpr.alliance_id = previous.alliance_id
        AND cpr.effective_date < latest.effective_date
        AND cpr.effective_date > previous.effective_date)
    )
/

