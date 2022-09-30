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

create or replace view STFC.V_ALLIANCE_SERVICE_COSTS as
SELECT aserv.id, zs.zone_id, z.owner_id alliance_id, aserv.LEVEL_ID, z.name Zone, zs.name Service, zs.description,
    nvl(ciso1.cost, 0) refined_iso1,
    nvl(ciso2.cost, 0) refined_iso2,
    nvl(ciso3.cost, 0) refined_iso3,
    nvl(cpe.cost, 0) Progenitor_Emitters,
    nvl(cpd.cost, 0) Progenitor_Diodes,
    nvl(cpc.cost, 0) Progenitor_Cores,
    nvl(cpr.cost, 0) Progenitor_Reactors
FROM ZONES z
INNER JOIN ZONE_SERVICES zs ON (z.ID = zs.ZONE_ID)
INNER JOIN ALLIANCE_SERVICES aserv ON (aserv.ZONE_SERVICE_ID = zs.ID AND aserv.ALLIANCE_ID = z.owner_id)
LEFT OUTER JOIN ZONE_SERVICE_COST ciso1 ON (zs.id = ciso1.service_id AND ciso1.resource_id = 51)
LEFT OUTER JOIN ZONE_SERVICE_COST ciso2 ON (zs.id = ciso2.service_id AND ciso2.resource_id = 52)
LEFT OUTER JOIN ZONE_SERVICE_COST ciso3 ON (zs.id = ciso3.service_id AND ciso3.resource_id = 53)
LEFT OUTER JOIN ZONE_SERVICE_COST cpe ON (zs.id = cpe.service_id AND cpe.resource_id = 61)
LEFT OUTER JOIN ZONE_SERVICE_COST cpd ON (zs.id = cpd.service_id AND cpd.resource_id = 62)
LEFT OUTER JOIN ZONE_SERVICE_COST cpc ON (zs.id = cpc.service_id AND cpc.resource_id = 63)
LEFT OUTER JOIN ZONE_SERVICE_COST cpr ON (zs.id = cpr.service_id AND cpr.resource_id = 64)
/

