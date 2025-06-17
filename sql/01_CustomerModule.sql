CREATE TABLE customers (
    id uuid NOT NULL,
    username text NOT NULL,
    email text NOT NULL,
    identity_id uuid NOT NULL,
    created_by text NOT NULL,
    created_date timestamp with time zone NOT NULL,
    last_modified_by text NOT NULL,
    last_modified_date timestamp with time zone,
    CONSTRAINT pk_customers PRIMARY KEY (id)
);

INSERT INTO customers (
	id, username, email, identity_id, created_by, created_date, last_modified_by, last_modified_date)
	VALUES (gen_random_uuid(), 'admin@cortado.com', 'admin@cortado.com', '59fec4c8-30b1-7099-efe3-0f9b3a241617', 'system', NOW(), 'system', NOW());