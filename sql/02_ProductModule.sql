CREATE TABLE products
(
    id UUID NOT NULL,
    customer_id UUID NOT NULL,
    code VARCHAR(20) NOT NULL,
    name VARCHAR(40) NOT NULL,
    description TEXT NOT NULL,
    image_url VARCHAR(255) NOT NULL,
    address VARCHAR(255) NOT NULL,
    start_date DATE NOT NULL,
    end_date DATE NOT NULL,
    start_time TIME NOT NULL,
    end_time TIME NOT NULL,
    registration_url TEXT NOT NULL,
    created_by VARCHAR(40) NOT NULL,
    created_date TIMESTAMP WITH TIME ZONE NOT NULL,
    last_modified_by VARCHAR(40) NOT NULL,
    last_modified_date TIMESTAMP WITH TIME ZONE,
    CONSTRAINT pk_products PRIMARY KEY(id),
    CONSTRAINT fk_products_customers_id FOREIGN KEY(customer_id) REFERENCES customers(id) ON DELETE CASCADE
);

CREATE TABLE product_items
(
    id UUID NOT NULL,
    name VARCHAR(40) NOT NULL,
    description TEXT NOT NULL,
    variants VARCHAR(40) NOT NULL,
    is_free BOOLEAN NOT NULL,
    unit_price DECIMAL NOT NULL,
    product_id UUID NOT NULL,
    created_by VARCHAR(40) NOT NULL,
    created_date TIMESTAMP WITH TIME ZONE NOT NULL,
    last_modified_by VARCHAR(40) NOT NULL,
    last_modified_date TIMESTAMP WITH TIME ZONE,
    CONSTRAINT pk_product_items PRIMARY KEY(id),
    CONSTRAINT fk_product_items_products_id FOREIGN KEY(product_id) REFERENCES products(id) ON DELETE CASCADE
);