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

create or replace view STFC.V_ALLIANCE_INVENTORY as
SELECT ai.id, ai.alliance_id, ai.effective_date,
    nvl(iiso1.amount, 0) refined_iso1,
    nvl(iiso2.amount, 0) refined_iso2,
    nvl(iiso3.amount, 0) refined_iso3,
    nvl(ipe.amount, 0) Progenitor_Emitters,
    nvl(ipd.amount, 0) Progenitor_Diodes,
    nvl(ipc.amount, 0) Progenitor_Cores,
    nvl(ipr.amount, 0) Progenitor_Reactors,
    nvl(icp.amount, 0) Collisional_Plasma,
    nvl(imp.amount, 0) Magnetic_Plasma,
    nvl(iss.amount, 0) Subspace_Superconductor,
    nvl(iar.amount, 0) Alliance_Reserves
FROM ALLIANCE_INVENTORY ai
LEFT OUTER JOIN ALLIANCE_INVENTORY_LINE iiso1 ON (ai.id = iiso1.alliance_inventory_id AND iiso1.resource_id = 51)
LEFT OUTER JOIN ALLIANCE_INVENTORY_LINE iiso2 ON (ai.id = iiso2.alliance_inventory_id AND iiso2.resource_id = 52)
LEFT OUTER JOIN ALLIANCE_INVENTORY_LINE iiso3 ON (ai.id = iiso3.alliance_inventory_id AND iiso3.resource_id = 53)
LEFT OUTER JOIN ALLIANCE_INVENTORY_LINE ipe ON (ai.id = ipe.alliance_inventory_id AND ipe.resource_id = 61)
LEFT OUTER JOIN ALLIANCE_INVENTORY_LINE ipd ON (ai.id = ipd.alliance_inventory_id AND ipd.resource_id = 62)
LEFT OUTER JOIN ALLIANCE_INVENTORY_LINE ipc ON (ai.id = ipc.alliance_inventory_id AND ipc.resource_id = 63)
LEFT OUTER JOIN ALLIANCE_INVENTORY_LINE ipr ON (ai.id = ipr.alliance_inventory_id AND ipr.resource_id = 64)
LEFT OUTER JOIN ALLIANCE_INVENTORY_LINE icp ON (ai.id = icp.alliance_inventory_id AND icp.resource_id = 71)
LEFT OUTER JOIN ALLIANCE_INVENTORY_LINE imp ON (ai.id = imp.alliance_inventory_id AND imp.resource_id = 72)
LEFT OUTER JOIN ALLIANCE_INVENTORY_LINE iss ON (ai.id = iss.alliance_inventory_id AND iss.resource_id = 73)
LEFT OUTER JOIN ALLIANCE_INVENTORY_LINE iar ON (ai.id = iar.alliance_inventory_id AND iar.resource_id = 74)
/

