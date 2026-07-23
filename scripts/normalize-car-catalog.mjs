import fs from "node:fs";
import path from "node:path";

const root = path.resolve(import.meta.dirname, "..");
const miotoPath = path.join(root, "VivuCarServer", "API", "SeedData", "mioto-cars.seed.json");
const catalogPath = path.join(root, "VivuCarServer", "BusinessObjects", "SeedData", "cloudinary-car-images.json");

const classifications = {
  "vinfast-vf3-2025": ["SUV", 4],
  "vinfast-vf3-2026": ["SUV", 4],
  "vinfast-vf7-eco-2026": ["SUV", 5],
  "vinfast-limo-green-2025": ["MPV", 7],
  "vinfast-vf5-2024": ["SUV", 5],
  "vinfast-vf6-eco-2024": ["SUV", 5],
  "vinfast-vf5-2025": ["SUV", 5],
  "vinfast-vf7-2025": ["SUV", 5],
  "vinfast-vf6-plus-2024": ["SUV", 5],
  "vinfast-minio-green-2026": ["Hatchback", 4],
  "chevrolet-spark-2012": ["Hatchback", 5],
  "kia-k3-premium-2022": ["Sedan", 5],
  "chevrolet-cruze-2015": ["Sedan", 5],
  "vinfast-lux-sa-2021": ["SUV", 7],
  "chevrolet-captiva-2016": ["SUV", 7],
  "mazda-3-luxury-2020": ["Sedan", 5],
  "chevrolet-colorado-4x2-2018": ["Pickup", 5],
  "omoda-c5-flagship-turbo-2025": ["SUV", 5],
  "mitsubishi-attrage-2020": ["Sedan", 5],
  "toyota-fortuner-2009": ["SUV", 7],
  "mitsubishi-outlander-2019": ["SUV", 7],
  "toyota-avanza-2023": ["MPV", 7],
  "hyundai-veloser-hatchback-2011": ["Hatchback", 4],
  "suzuki-swift-hatchback-2016": ["Hatchback", 5],
  "kia-sorento-premium-2018": ["SUV", 7],
  "honda-city-2016": ["Sedan", 5],
  "hyundai-accent-2020": ["Sedan", 5],
  "kia-soluto-2021": ["Sedan", 5],
  "mercedes-c200-2008": ["Sedan", 5],
  "mitsubishi-mirage-2018": ["Hatchback", 5],
  "kia-sonet-luxury-2025": ["SUV", 5],
  "mitsubishi-outlander-2022": ["SUV", 7],
  "mazda-3-premium-2017": ["Sedan", 5],
  "peugeot-3008-2018": ["SUV", 5],
  "hyundai-creta-luxury-2025": ["SUV", 5],
  "ford-territory-titanium-2023": ["SUV", 5],
  "mazda-cx8-premium-2023": ["SUV", 6],
  "mitsubishi-outlander-premium-2020": ["SUV", 7],
  "hyundai-venue-2025": ["SUV", 5],
  "ford-ranger-xls-4x4-2025": ["Pickup", 5],
  "suzuki-ciaz-2017": ["Sedan", 5],
  "honda-city-2018": ["Sedan", 5],
  "mazda-2-2025": ["Sedan", 5],
  "kia-sportage-signature-2025": ["SUV", 5],
  "hyundai-venue-2024": ["SUV", 5],
  "kia-carens-2024": ["MPV", 7],
  "nissan-almera-el-2022": ["Sedan", 5],
  "honda-jazz-2018": ["Hatchback", 5],
  "honda-crv-l-awd-2018": ["SUV", 7],
  "mitsubishi-xpander-2026": ["MPV", 7],
  "ford-focus-2017": ["Sedan", 5],
  "hyundai-creta-luxury-2023": ["SUV", 5],
  "mazda-6-premium-2021": ["Sedan", 5],
  "mitsubishi-xforce-ultimate-2025": ["SUV", 5],
  "toyota-corolla-cross-v-2025": ["SUV", 5],
  "geely-coolray-standard-2025": ["SUV", 5],
  "mitsubishi-pajero-2018": ["SUV", 7],
  "honda-hrv-g-2023": ["SUV", 5],
  "honda-crv-l-2022": ["SUV", 7],
  "kia-cerato-2020": ["Sedan", 5],
  "vinfast-limo-green-2026": ["MPV", 7],
  "suzuki-ertiga-2020": ["MPV", 7],
  "kia-carnival-premium-2024": ["MPV", 8],
  "vinfast-vf6-plus-2025": ["SUV", 5],
  "vinfast-vf7-plus-2025": ["SUV", 5],
  "mercedes-c200-exclusive-2023": ["Sedan", 5],
  "mercedes-c250-2016": ["Sedan", 5],
  "toyota-yaris-cross-2024": ["SUV", 5],
  "hyundai-accent-2012": ["Sedan", 5]
};

const modelOverrides = {
  "vinfast-vf3-2025": "VF 3",
  "vinfast-vf3-2026": "VF 3",
  "vinfast-vf5-2024": "VF 5",
  "vinfast-vf5-2025": "VF 5",
  "vinfast-vf6-eco-2024": "VF 6 Eco",
  "vinfast-vf6-plus-2024": "VF 6 Plus",
  "vinfast-vf7-2025": "VF 7",
  "vinfast-vf7-eco-2026": "VF 7 Eco",
  "hyundai-veloser-hatchback-2011": "Veloster",
  "mazda-cx8-premium-2023": "CX-8 Premium",
  "ford-ranger-xls-4x4-2025": "Ranger XLS 4x4",
  "honda-crv-l-awd-2018": "CR-V L AWD",
  "honda-hrv-g-2023": "HR-V G",
  "honda-crv-l-2022": "CR-V L",
  "vinfast-vf6-plus-2025": "VF 6 Plus",
  "vinfast-vf7-plus-2025": "VF 7 Plus"
};

const brandOverrides = {
  Vinfast: "VinFast",
  Mercedes: "Mercedes-Benz"
};

const fuelOverrides = {
  "chevrolet-colorado-4x2-2018": "diesel"
};

function repairUtf8(value) {
  if (typeof value !== "string" || !/[ÆÃÄÅáºá»]/.test(value)) return value;
  return Buffer.from(value, "latin1").toString("utf8");
}

function sourceNumber(sourceOrder) {
  return Number(String(sourceOrder).split("-").at(-1));
}

const mioto = JSON.parse(fs.readFileSync(miotoPath, "utf8"));
const catalog = JSON.parse(fs.readFileSync(catalogPath, "utf8"));
const sourceByOrder = new Map(mioto.cars.map(car => [sourceNumber(car.source_order), car]));

for (const catalogCar of catalog.cars) {
  const source = sourceByOrder.get(catalogCar.source_order);
  const classification = classifications[catalogCar.slug];
  if (!source || !classification) {
    throw new Error(`Missing source or classification for ${catalogCar.slug}`);
  }

  const brand = brandOverrides[source.brand] ?? source.brand.trim();
  const model = modelOverrides[catalogCar.slug] ?? source.model.trim().replace(/\s+/g, " ");
  const [carType, seats] = classification;
  const fuelType = fuelOverrides[catalogCar.slug] ?? source.fuel_type;
  const address = repairUtf8(source.address).replace(/\s+/g, " ").trim();

  Object.assign(source, {
    title: `${brand} ${model} ${source.year}`,
    brand,
    model,
    car_type: carType,
    seats,
    kilometers_driven: 0,
    fuel_type: fuelType,
    price_per_hours: Math.round(source.price_per_day / 10),
    address,
    description: "",
    status: "available"
  });

  Object.assign(catalogCar, {
    name: source.title,
    brand,
    model,
    year: source.year,
    car_type: carType,
    seats,
    kilometers_driven: 0,
    transmission: source.transmission,
    fuel_type: fuelType,
    price_per_day: source.price_per_day,
    address,
    description: "",
    status: "available",
    color: null
  });
}

const catalogOrders = new Set(catalog.cars.map(car => car.source_order));
mioto.cars = mioto.cars.filter(car => catalogOrders.has(sourceNumber(car.source_order)));
mioto.count = mioto.cars.length;
catalog.car_count = catalog.cars.length;
catalog.image_count = catalog.cars.reduce((total, car) => total + car.images.length, 0);

fs.writeFileSync(miotoPath, `${JSON.stringify(mioto, null, 2)}\n`, "utf8");
fs.writeFileSync(catalogPath, `${JSON.stringify(catalog, null, 2)}\n`, "utf8");

console.log(`Normalized ${catalog.cars.length} cars and ${catalog.image_count} images.`);
