import { apiFetch, fetchJson, sendJson } from "../../shared/api-client.js";
export function getVouchers(params={}){const query=new URLSearchParams();Object.entries(params).forEach(([key,value])=>{if(value!==""&&value!=null)query.set(key,value)});return fetchJson(`admin/vouchers${query.size?`?${query}`:""}`)}
export function getVoucher(id){return fetchJson(`admin/vouchers/${id}`)}
export function getVoucherPerformance(id){return fetchJson(`admin/vouchers/${id}/performance`)}
export function saveVoucher(data,id){return sendJson(id?`admin/vouchers/${id}`:"admin/vouchers",data,{method:id?"PUT":"POST"})}
export async function deleteVoucher(id){const response=await apiFetch(`admin/vouchers/${id}`,{method:"DELETE"});if(!response.ok){const payload=await response.json().catch(()=>null);throw new Error(payload?.message||"Khong the xoa voucher.")}}
