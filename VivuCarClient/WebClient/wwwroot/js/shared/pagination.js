export function paginate(items, page = 1, pageSize = 10) {
    const totalItems = items.length;
    const totalPages = Math.max(1, Math.ceil(totalItems / pageSize));
    const currentPage = Math.min(Math.max(1, page), totalPages);
    const start = (currentPage - 1) * pageSize;

    return {
        items: items.slice(start, start + pageSize),
        page: currentPage,
        pageSize,
        totalItems,
        totalPages,
        start: totalItems ? start + 1 : 0,
        end: Math.min(start + pageSize, totalItems)
    };
}