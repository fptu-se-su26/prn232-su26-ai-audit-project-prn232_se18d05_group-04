export function mapCarResponse(item) {
    if (!item) return null;

    return {
        id: item.id ?? item.Id,
        owner_id: item.owner_id ?? item.ownerId ?? item.OwnerId,
        owner_name: item.owner_name ?? item.ownerName ?? item.OwnerName ?? "-",
        brand: item.brand ?? item.Brand ?? "",
        model: item.model ?? item.Model ?? "",
        car_model_id: item.car_model_id ?? item.carModelId ?? item.CarModelId ?? null,
        type_id: item.type_id ?? item.typeId ?? item.TypeId ?? null,
        type_name: item.type_name ?? item.typeName ?? item.TypeName ?? "",
        license_plate: item.license_plate ?? item.licensePlate ?? item.LicensePlate ?? "",
        year: item.year ?? item.Year ?? null,
        color: item.color ?? item.Color ?? "",
        seats: item.seats ?? item.Seats ?? null,
        kilometers_driven: item.kilometers_driven ?? item.kilometersDriven ?? item.KilometersDriven ?? 0,
        transmission: item.transmission ?? item.Transmission ?? "automatic",
        fuel_type: item.fuel_type ?? item.fuelType ?? item.FuelType ?? "gasoline",
        price_per_day: item.price_per_day ?? item.pricePerDay ?? item.PricePerDay ?? 0,
        price_per_hours: item.price_per_hours ?? item.pricePerHours ?? item.PricePerHours ?? 0,
        address: item.address ?? item.Address ?? "",
        description: item.description ?? item.Description ?? "",
        status: item.status ?? item.Status ?? "available",
        blocked_reason: item.blocked_reason ?? item.blockedReason ?? item.BlockedReason ?? null,
        primary_image_url: item.primary_image_url ?? item.primaryImageUrl ?? item.PrimaryImageUrl ?? null,
        images: item.images ?? item.Images ?? [],
        created_at: item.created_at ?? item.createdAt ?? item.CreatedAt ?? null,
        updated_at: item.updated_at ?? item.updatedAt ?? item.UpdatedAt ?? null
    };
}

export function mapCarListResponse(payload) {
    if (Array.isArray(payload)) {
        return {
            items: payload.map(mapCarResponse).filter(Boolean),
            page: 1,
            pageSize: payload.length,
            totalItems: payload.length,
            totalPages: 1
        };
    }

    const items = payload?.items ?? payload?.Items ?? [];

    return {
        items: items.map(mapCarResponse).filter(Boolean),
        page: payload?.page ?? payload?.Page ?? payload?.currentPage ?? payload?.CurrentPage ?? 1,
        pageSize: payload?.page_size ?? payload?.pageSize ?? payload?.PageSize ?? items.length,
        totalItems: payload?.total_items ?? payload?.totalItems ?? payload?.TotalItems ?? items.length,
        totalPages: payload?.total_pages ?? payload?.totalPages ?? payload?.TotalPages ?? 1
    };
}
