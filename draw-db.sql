-- ============================================================
-- VivuCar Database Schema
-- ============================================================

-- ============================================================
-- SECTION 1: AUTHENTICATION & USER MANAGEMENT
-- ============================================================

CREATE TABLE users (
    id              	BIGSERIAL PRIMARY KEY,
    google_id       	VARCHAR(100) UNIQUE,
    email           	VARCHAR(255) NOT NULL UNIQUE,
    full_name       	VARCHAR(255) NOT NULL,
    avatar_url      	TEXT,
    role            	VARCHAR(20) NOT NULL
	                    DEFAULT 'user'
	                    CHECK (
	                        role IN (
	                            'user',
	                            'car_owner',
	                            'admin'
	                        )
	                    ),
    is_blocked      	BOOLEAN DEFAULT FALSE,
    created_at      	TIMESTAMPTZ DEFAULT NOW()
);

CREATE TABLE user_documents (
    id              	BIGSERIAL PRIMARY KEY,
    user_id         	BIGINT NOT NULL
	                    REFERENCES users(id)
	                    ON DELETE CASCADE,
    document_type       VARCHAR(50) NOT NULL
	                        CHECK (
	                            document_type IN (
	                                'avatar',
	                                'license_front',
	                                'license_back'
	                            )
	                        ),
    file_name   		VARCHAR(50) NOT NULL
    file_url        	TEXT NOT NULL,
    verified        	BOOLEAN DEFAULT FALSE,
    created_at      	TIMESTAMPTZ DEFAULT NOW()
);

-- ============================================================
-- SECTION 2: CAR MANAGEMENT
-- ============================================================

CREATE TABLE car_types (
    id              	SERIAL PRIMARY KEY,
    name           		VARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE cars (
    id                  SERIAL PRIMARY KEY,
    owner_id            BIGINT NOT NULL
                        REFERENCES users(id)
                        ON DELETE CASCADE,
    brand               VARCHAR(100) NOT NULL,
    model               VARCHAR(100) NOT NULL,
    type_id             INT
                        REFERENCES car_types(id),
    license_plate       VARCHAR(20) NOT NULL UNIQUE,
    year                SMALLINT
                        CHECK (year >= 1900),
    color               VARCHAR(50),
    seats               SMALLINT
                        CHECK (seats > 0),
   kilometers_driven INT check(kilometers_driven > 0),
    transmission        VARCHAR(20)
                        CHECK (
                            transmission IN (
                                'manual',
                                'automatic',
                                'cvt'
                            )
                        ),
    fuel_type           VARCHAR(20)
                        CHECK (
                            fuel_type IN (
                                'gasoline',
                                'diesel',
                                'electric',
                                'hybrid'
                            )
                        ),
    price_per_day       DECIMAL(10,2) NOT NULL
                        CHECK (price_per_day > 0),
    price_per_hours       DECIMAL(10,2) NOT NULL
                        CHECK (price_per_hours  > 0),
    address             TEXT NOT NULL,
    description         TEXT,
    status              VARCHAR(20)
                        DEFAULT 'available'
                        CHECK (
                            status IN (
                                'available',
                                'rented',
                                'maintenance',
                                'blocked'
                            )
                        ),
    blocked_reason      TEXT,
    created_at          TIMESTAMPTZ DEFAULT NOW()
);

CREATE TABLE car_images (
    id                  SERIAL PRIMARY KEY,
    car_id              INT NOT NULL
                        REFERENCES cars(id)
                        ON DELETE CASCADE,
    image_url           VARCHAR(500) NOT NULL,
    is_primary          BOOLEAN DEFAULT FALSE
);

-- ============================================================
-- SECTION 3: VOUCHERS & PROMOTIONS
-- ============================================================

CREATE TABLE vouchers (
    id                      SERIAL PRIMARY KEY,
    name VARCHAR(50)
    code                    VARCHAR(50) NOT NULL UNIQUE,
    discount_type           VARCHAR(20) NOT NULL
                            CHECK (
                                discount_type IN (
                                    'percentage',
                                    'fixed'
                                )
                            ),
    discount_value          DECIMAL(10,2) NOT NULL
                            CHECK (discount_value > 0),
    min_order_amount        DECIMAL(10,2)
                            DEFAULT 0
                            CHECK (min_order_amount >= 0),
    max_discount 			DECIMAL(10,2) NOT NULL
                            CHECK (max_discount  > 0),
    quantity                INT DEFAULT 0
                            CHECK (quantity >= 0),
    expires_at              TIMESTAMPTZ,
    created_at              TIMESTAMPTZ DEFAULT NOW()
);


CREATE TABLE voucher_usages (
    id                      SERIAL PRIMARY KEY,
    voucher_id              INT NOT NULL
                            REFERENCES vouchers(id)
                            ON DELETE CASCADE,
    user_id                 BIGINT NOT NULL
                            REFERENCES users(id)
                            ON DELETE CASCADE,
    booking_id              INT
                            REFERENCES bookings(id)
                            ON DELETE SET NULL,
    used_at                 TIMESTAMPTZ DEFAULT NOW(),
    UNIQUE(voucher_id, user_id, booking_id)
);

-- ============================================================
-- SECTION 4: BOOKINGS
-- ============================================================

CREATE TABLE bookings (
    id                      SERIAL PRIMARY KEY,
    user_id             BIGINT NOT NULL
                            REFERENCES users(id)
                            ON DELETE CASCADE,
    car_id                  INT NOT NULL
                            REFERENCES cars(id)
                            ON DELETE CASCADE,
    pickup_datetime         TIMESTAMPTZ NOT NULL,
    return_datetime         TIMESTAMPTZ NOT NULL,
    pickup_address          TEXT,
    total_amount            DECIMAL(10,2) NOT NULL
                            CHECK (total_amount > 0),
    voucher_id              INT
                            REFERENCES vouchers(id)
                            ON DELETE SET NULL,
    status                  VARCHAR(20)
                            DEFAULT 'pending'
                            CHECK (
                                status IN (
                                    'pending',
                                    'approved',
                                    'rejected',
                                    'completed',
                                    'cancelled'
                                )
                            ),

    created_at              TIMESTAMPTZ DEFAULT NOW(),
    CONSTRAINT valid_booking_dates
    CHECK (return_datetime > pickup_datetime)
);

-- ============================================================
-- SECTION 5: PAYMENTS
-- ============================================================

CREATE TABLE payments (
    id                      SERIAL PRIMARY KEY,
    booking_id              INT NOT NULL
                            REFERENCES bookings(id)
                            ON DELETE CASCADE,
    method                  VARCHAR(30)
                            CHECK (
                                method IN (
                                    'vnpay',
                                    'momo',
                                    'cash'
                                )
                            ),
    amount                  DECIMAL(10,2) NOT NULL
                            CHECK (amount > 0),
    status                  VARCHAR(20)
                            DEFAULT 'pending'
                            CHECK (
                                status IN (
                                    'pending',
                                    'success',
                                    'failed',
                                    'refunded'
                                )
                            ),

    transaction_code        VARCHAR(255),
    paid_at                 TIMESTAMPTZ
);

-- ============================================================
-- SECTION 6: NOTIFICATIONS
-- ============================================================

CREATE TABLE notifications (
    id                      SERIAL PRIMARY KEY,
    user_id                 BIGINT NOT NULL
                            REFERENCES users(id)
                            ON DELETE CASCADE,
    title                   VARCHAR(255) NOT NULL,
    content                 TEXT NOT NULL,
    is_read                 BOOLEAN DEFAULT FALSE,
    created_at              TIMESTAMPTZ DEFAULT NOW()
);

-- ============================================================
-- SECTION 7: HANDOVER & INSPECTION
-- ============================================================

CREATE TABLE handover_inspections (
    id                      SERIAL PRIMARY KEY,
    booking_id              INT NOT NULL
                            REFERENCES bookings(id)
                            ON DELETE CASCADE,
    inspected_by            BIGINT NOT NULL
                            REFERENCES users(id),
    inspection_type         VARCHAR(20) NOT NULL
                            CHECK (
                                inspection_type IN (
                                    'pre_rental',
                                    'post_rental'
                                )
                            ),
    odometer_km             INT
                            CHECK (odometer_km >= 0),
    damage_notes            TEXT,
    note                    TEXT,
    confirmed_at            TIMESTAMPTZ DEFAULT NOW()
);

CREATE TABLE inspection_images (
    id                      SERIAL PRIMARY KEY,
    inspection_id           INT NOT NULL
                            REFERENCES handover_inspections(id)
                            ON DELETE CASCADE,
    url                     VARCHAR(500) NOT NULL,
    caption                 TEXT,
    uploaded_at             TIMESTAMPTZ DEFAULT NOW()
);

-- ============================================================
-- SECTION 8: REVIEWS & RATINGS
-- ============================================================

CREATE TABLE reviews (
    id                      SERIAL PRIMARY KEY,
    booking_id              INT NOT NULL
                            REFERENCES bookings(id)
                            ON DELETE CASCADE,
    reviewer_id             BIGINT NOT NULL
                            REFERENCES users(id)
                            ON DELETE CASCADE,
    car_id                  INT NOT NULL
                            REFERENCES cars(id)
                            ON DELETE CASCADE,
    rating                  SMALLINT NOT NULL
                            CHECK (rating BETWEEN 1 AND 5),
    comment                 TEXT,
    created_at              TIMESTAMPTZ DEFAULT NOW(),
    UNIQUE (booking_id, reviewer_id)
);

-- ============================================================
-- SECTION 9: INCIDENT REPORTS
-- ============================================================

CREATE TABLE incident_reports (
    id                      SERIAL PRIMARY KEY,
    booking_id              INT NOT NULL
                            REFERENCES bookings(id)
                            ON DELETE CASCADE,
    reported_by             BIGINT NOT NULL
                            REFERENCES users(id)
                            ON DELETE CASCADE,
    title                   VARCHAR(255) NOT NULL,
    description             TEXT NOT NULL,
    status                  VARCHAR(30)
                            DEFAULT 'open'
                            CHECK (
                                status IN (
                                    'open',
                                    'in_review',
                                    'resolved',
                                    'closed'
                                )
                            ),
    created_at              TIMESTAMPTZ DEFAULT NOW()
);

CREATE TABLE incident_images (
    id                      SERIAL PRIMARY KEY,
    incident_id             INT NOT NULL
                            REFERENCES incident_reports(id)
                            ON DELETE CASCADE,
    url                     VARCHAR(500) NOT NULL,
    caption                 TEXT,
    uploaded_at             TIMESTAMPTZ DEFAULT NOW()
);

-- ============================================================
-- SECTION 10: AI CHATBOT & SUPPORT
-- ============================================================

CREATE TABLE chat_sessions (
    id                      SERIAL PRIMARY KEY,
    user_id                 BIGINT NOT NULL
                            REFERENCES users(id)
                            ON DELETE CASCADE,
    booking_id              INT
                            REFERENCES bookings(id)
                            ON DELETE SET NULL,
    session_type            VARCHAR(20)
                            DEFAULT 'ai'
                            CHECK (
                                session_type IN (
                                    'ai',
                                    'live'
                                )
                            ),
    status                  VARCHAR(20)
                            DEFAULT 'open'
                            CHECK (
                                status IN (
                                    'open',
                                    'escalated',
                                    'closed'
                                )
                            ),
    assigned_to             BIGINT
                            REFERENCES users(id)
                            ON DELETE SET NULL,
    escalated_at            TIMESTAMPTZ,
    closed_at               TIMESTAMPTZ,
    created_at              TIMESTAMPTZ DEFAULT NOW()
);

CREATE TABLE chat_messages (
    id                      SERIAL PRIMARY KEY,
    session_id              INT NOT NULL
                            REFERENCES chat_sessions(id)
                            ON DELETE CASCADE,
    sender_id               BIGINT
                            REFERENCES users(id)
                            ON DELETE SET NULL,
    role                    VARCHAR(20) NOT NULL
                            CHECK (
                                role IN (
                                    'user',
                                    'assistant',
                                    'system'
                                )
                            ),

    content                 TEXT NOT NULL,
    created_at              TIMESTAMPTZ DEFAULT NOW()
);

-- ============================================================
-- SECTION 11: REPORTING & EXPORT
-- ============================================================

CREATE TABLE daily_revenue_snapshots (
    id                      SERIAL PRIMARY KEY,
    snapshot_date           DATE NOT NULL UNIQUE,
    total_bookings          INT DEFAULT 0,
    completed_bookings      INT DEFAULT 0,
    cancelled_bookings      INT DEFAULT 0,
    gross_revenue           DECIMAL(14,2) DEFAULT 0,
    net_revenue             DECIMAL(14,2) DEFAULT 0,
    deposit_collected       DECIMAL(14,2) DEFAULT 0,
    generated_at            TIMESTAMPTZ DEFAULT NOW()
);

CREATE TABLE export_jobs (
    id                      SERIAL PRIMARY KEY,
    requested_by            BIGINT NOT NULL
                            REFERENCES users(id)
                            ON DELETE CASCADE,
    export_type             VARCHAR(30) NOT NULL,
    params                  JSONB,
    status                  VARCHAR(20)
                            DEFAULT 'pending'
                            CHECK (
                                status IN (
                                    'pending',
                                    'processing',
                                    'done',
                                    'failed'
                                )
                            ),
    file_url                VARCHAR(500),
    error_message           TEXT,
    created_at              TIMESTAMPTZ DEFAULT NOW(),
    completed_at            TIMESTAMPTZ
);


