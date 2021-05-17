CREATE TABLE foo (
    pk uuid,
    ck blob,
    val text,
    PRIMARY KEY (pk, ck)
);

INSERT INTO foo (pk, ck, val) VALUES (fe474926-f997-4a42-b2d9-d83e6d2fb2cf, 0XABCDEF, 'Test');
