CREATE TABLE products
(
    id uuid NOT NULL,
    code text NOT NULL,
    name text NOT NULL,
    description text NOT NULL,
    image_url text NOT NULL,
    address text NOT NULL,
    start_date date NOT NULL,
    end_date date NOT NULL,
    start_time time NOT NULL,
    end_time time NOT NULL,
    customer_id uuid NOT NULL,
    created_by text NOT NULL,
    created_date timestamp with time zone NOT NULL,
    last_modified_by text NOT NULL,
    last_modified_date timestamp with time zone,
    CONSTRAINT pk_products PRIMARY KEY(id),
    CONSTRAINT fk_products_customers_id FOREIGN KEY(customer_id) REFERENCES customers(id) ON DELETE CASCADE
);

CREATE TABLE product_items
(
    id uuid NOT NULL,
    name text NOT NULL,
    description text NOT NULL,
    variants text NOT NULL,
    is_free boolean NOT NULL,
    unit_price decimal NOT NULL,
    product_id uuid NOT NULL,
    created_by text NOT NULL,
    created_date timestamp with time zone NOT NULL,
    last_modified_by text NOT NULL,
    last_modified_date timestamp with time zone,
    CONSTRAINT pk_product_items PRIMARY KEY(id),
    CONSTRAINT fk_product_items_products_id FOREIGN KEY(product_id) REFERENCES products(id) ON DELETE CASCADE
);