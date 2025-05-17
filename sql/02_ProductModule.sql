CREATE TABLE products
(
    id UUID NOT NULL,
    customer_id UUID NOT NULL,
    code TEXT NOT NULL,
    name TEXT NOT NULL,
    description TEXT NOT NULL,
    image_url TEXT NOT NULL,
    address TEXT NOT NULL,
    start_date DATE NOT NULL,
    end_date DATE NOT NULL,
    start_time TIME NOT NULL,
    end_time TIME NOT NULL,
    registration_url TEXT NOT NULL,
    created_by TEXT NOT NULL,
    created_date TIMESTAMP WITH TIME ZONE NOT NULL,
    last_modified_by TEXT NOT NULL,
    last_modified_date TIMESTAMP WITH TIME ZONE,
    CONSTRAINT pk_products PRIMARY KEY(id),
    CONSTRAINT fk_products_customers_id FOREIGN KEY(customer_id) REFERENCES customers(id) ON DELETE CASCADE
);

CREATE TABLE product_items
(
    id UUID NOT NULL,
    name TEXT NOT NULL,
    description TEXT NOT NULL,
    variants TEXT NOT NULL,
    is_free BOOLEAN NOT NULL,
    unit_price DECIMAL NOT NULL,
    product_id UUID NOT NULL,
    created_by TEXT NOT NULL,
    created_date TIMESTAMP WITH TIME ZONE NOT NULL,
    last_modified_by TEXT NOT NULL,
    last_modified_date TIMESTAMP WITH TIME ZONE,
    CONSTRAINT pk_product_items PRIMARY KEY(id),
    CONSTRAINT fk_product_items_products_id FOREIGN KEY(product_id) REFERENCES products(id) ON DELETE CASCADE
);